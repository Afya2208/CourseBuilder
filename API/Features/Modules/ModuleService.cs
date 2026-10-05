using System.Net.Mime;
using API.Dto;
using ClosedXML.Excel;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.StaticFiles;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Modules;

public interface IModuleService
{
    Task<List<ModuleDto>> ReadAllCourseModulesAsync(int courseId);
    Task ImportModulesFromXlsxAsync(XlsxFile xlsxFile, int courseId);
    Task<ModuleDto> AddAsync(ModuleDto moduleDto);
    Task DeleteAsync(long moduleId);
    Task<ModuleDto> UpdateAsync(long moduleId, ModuleDto moduleDto);
    Task<ModuleDto> ReadByIdAsync(long moduleId);
    Task SaveModulesOrderAsync(IEnumerable<ModuleOrderDto> modulesOrder);
}

public class ModuleService(IModuleRepository moduleRepository) : IModuleService
{
    public async Task<List<ModuleDto>> ReadAllCourseModulesAsync(int courseId)
    {
        return await moduleRepository.ReadAllCourseModulesAsync(courseId);
    }

    public async Task ImportModulesFromXlsxAsync(XlsxFile xlsxFile, int courseId)
    {
        var fileExtProvider = new FileExtensionContentTypeProvider();
        fileExtProvider.TryGetContentType(".xlsx", out var type);
        if (xlsxFile.File.ContentType != type || !xlsxFile.File.FileName.EndsWith(".xlsx"))
        {
            throw new ArgumentException("Неверный формат и расширение файла.");
        }

        await using var fileStream = xlsxFile.File.OpenReadStream();
        using IXLWorkbook workbook = new XLWorkbook(fileStream);
        var sheet = workbook.Worksheets.First();
        if (!CheckTitles(sheet))
        {
            throw new ArgumentException(
                $"Неверный формат файла. Должны быть заголовки: Название, Описание, Порядок");
        }

        var rowIndex = 2;
        List<Module> modules = new();
        while (!string.IsNullOrWhiteSpace(sheet.Cell(rowIndex, 1).GetString())
               || !string.IsNullOrWhiteSpace(sheet.Cell(rowIndex, 2).GetString()))
        {
            var name = sheet.Cell(rowIndex, 1).GetString();
            var desc = sheet.Cell(rowIndex, 2).GetString();
            var order = (int)sheet.Cell(rowIndex, 3).GetDouble();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(desc))
            {
                throw new ArgumentException(
                    $"Неверный формат файла. Требуется указать название и описание модуля в строке №{rowIndex}");
            }

            var newModule = new Module()
            {
                CourseId = courseId,
                Name = name,
                Description = desc,
                LessonsHaveOrder = true,
                Order = order
            };
            modules.Add(newModule);
            rowIndex++;
        }
        await moduleRepository.AddRangeAsync(modules);
    }

    public bool CheckTitles(IXLWorksheet worksheet)
    {
        var nameTitle = worksheet.Cell(1, 1).GetString();
        var descTitle = worksheet.Cell(1, 2).GetString();
        var orderTitle = worksheet.Cell(1, 3).GetString();
        return nameTitle.EqualsCi("Название")
               && descTitle.EqualsCi("Описание")
               && orderTitle.EqualsCi("Порядок");
    }

    public async Task<ModuleDto> AddAsync(ModuleDto moduleDto)
    {
        var entity = moduleDto.MapToEntity();
        var saved = await moduleRepository.AddAsync(entity);
        return new ModuleDto(saved);
    }

    public async Task<ModuleDto> ReadByIdAsync(long moduleId)
    {
        var module = await moduleRepository.ReadByIdAsync(moduleId);
        if (module == null)
            throw new NotFoundException("Не найден модуль", moduleId);
        return new ModuleDto(module);
    }

    public async Task SaveModulesOrderAsync(IEnumerable<ModuleOrderDto> modulesOrder)
    {
        await moduleRepository.SaveModulesOrderAsync(modulesOrder);
    }

    public async Task DeleteAsync(long moduleId)
    {
        await moduleRepository.DeleteAsync(moduleId);
    }

    public async Task<ModuleDto> UpdateAsync(long moduleId, ModuleDto moduleDto)
    {
        var module = moduleDto.MapToEntity();
        if (module.Id != moduleId)
            throw new ArgumentException("Данные модуля не совпадают с переданным идентификатором");
        var saved = await moduleRepository.UpdateAsync(moduleId, module);
        return new ModuleDto(saved);
    }
}