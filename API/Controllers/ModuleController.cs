using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class ModuleController(ModuleRepository moduleRepository, LessonRepository lessonRepository,
        CoursesDbContext context) : ControllerBase
    {
        [HttpGet("courses/{courseId:int}/modules")]
        public async Task<IActionResult> FindAllByCourseId(int courseId)
        {
            List<Module> foundModules = await moduleRepository.FindAllByConditionAsync(x => x.CourseId == courseId);
            List<ModuleDto> moduleDtos = foundModules.ConvertAll(x=>x.ToDto());
            moduleDtos.ForEach(async x=>x.LessonsCount = await lessonRepository.GetLessonsCountForModule(x.Id));
            moduleDtos = moduleDtos.OrderBy(x=> x.Order).ToList();
            return Ok(moduleDtos);
        }
        [HttpGet("modules/{moduleId:long}")]
        public async Task<IActionResult> FindById(long moduleId)
        {
            Module? module = await moduleRepository.FindByIdAsync(moduleId);
            if (module == null) throw new NotFoundException($"Не найден модуль id={module}");
            return Ok(module.ToDto());
        }
        [HttpGet("modules/{moduleId:long}/have-order")]
        public async Task<IActionResult> Lessons(long moduleId)
        {
            return Ok((await context.Modules.FindAsync(moduleId)).LessonsHaveOrder);
        }
        [HttpPost("modules")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Add([FromBody] ModuleDto moduleToAdd)
        {
            return Ok((await moduleRepository.AddAsync(moduleToAdd.ToEntity())).ToDto());
        }
        [HttpPost("course/{courseId:int}/import-modules")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> ImportModules([FromForm] XlsxFile xlsxFile, int courseId)
        {
            return Ok(await moduleRepository.ImportXlsx(xlsxFile.File.OpenReadStream(), courseId));
        }
        
        [HttpPost("/modules/order")]
        public async Task<IActionResult> SaveModulesOrder([FromBody] List<ModuleOrder> modulesOrder)
        {
            foreach (var moduleOrder in modulesOrder)
            {
                var module = await context.Modules.FindAsync(moduleOrder.Id);
                module.Order = moduleOrder.Order;
            }
            await context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPut("modules")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Update([FromBody] ModuleDto moduleToUpdate)
        {
            return Ok((await moduleRepository.UpdateAsync(moduleToUpdate.ToEntity())).ToDto());
        }
        [HttpDelete("modules/{moduleId:long}")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Delete(long moduleId)
        {
            var module = await moduleRepository.DeleteAsync(moduleId);
            if (module == null) return NoContent();
            return Ok(module.ToDto());
        }
    }
}