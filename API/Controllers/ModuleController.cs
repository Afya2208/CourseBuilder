using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class ModuleController(ModuleRepository moduleRepository, LessonRepository lessonRepository) : ControllerBase
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
    }
}