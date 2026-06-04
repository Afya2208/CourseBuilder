using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Models.Entities;

namespace API.Repositories
{
    public class ModuleRepository(CoursesDbContext context) : BaseRepository<Module>(context)
    {
        public async Task<int?> GetModulesCountForCourse(int courseId)
        {
            return context.Modules.Count(x=>x.CourseId == courseId);
        }

        public async Task<bool> ImportXlsx(Stream memoryStream, int courseId)
        {
            using (IXLWorkbook workbook = new XLWorkbook(memoryStream))
            {
                var sheet = workbook.Worksheets.First();
                if (CheckTitles(sheet))
                {
                    var rowIndex = 2;
                    while (!string.IsNullOrWhiteSpace(sheet.Cell(rowIndex, 1).GetString()) 
                    || !string.IsNullOrWhiteSpace(sheet.Cell(rowIndex, 2).GetString()))
                    {
                        var name = sheet.Cell(rowIndex, 1).GetString();
                        var desc = sheet.Cell(rowIndex, 2).GetString();
                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(desc))
                        {
                            throw new ArgumentException($"Требуется указать название и описание модуля в строке №{rowIndex}");
                        }
                        var order = (int)sheet.Cell(rowIndex, 3).GetDouble();
                        var newModule = new Module()
                        {
                            CourseId = courseId,
                            Name = name,
                            Description = desc,
                            LessonsHaveOrder = true,
                            Order = order
                        };
                        await context.Modules.AddAsync(newModule);
                        rowIndex++;
                    }
                    await context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new ArgumentException($"Неправильный формат файла, заголовки должны быть: Название, Описание, Порядок в курсе");
                }
            }
        }

        public bool CheckTitles(IXLWorksheet worksheet)
        {
            var nameTitle = worksheet.Cell(1, 1).GetString();
            var descTitle = worksheet.Cell(1, 2).GetString();
            var orderTitle = worksheet.Cell(1, 3).GetString();
            return nameTitle == "Название" && descTitle == "Описание" && orderTitle == "Порядок в курсе";
        }
    }
}