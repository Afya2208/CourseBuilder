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
    public class LessonController(LessonRepository lessonRepository) : ControllerBase
    {

        [HttpGet("lessons/{lessonId:long}")]
        public async Task<IActionResult> FindById(long lessonId)
        {
            Lesson? lesson = await lessonRepository.FindByIdAsync(lessonId);
            if (lesson == null) throw new NotFoundException($"Не найдено занятие id={lessonId}");
            return Ok(lesson.ToDto());
        }
        [HttpGet("modules/{moduleId:long}/lessons")]
        public async Task<IActionResult> FindAllByModuleId(long moduleId)
        {
            List<Lesson> foundLessons = await lessonRepository.FindAllByConditionAsync(x => x.ModuleId == moduleId);
            List<LessonDto> lessonDtos = foundLessons.ConvertAll(x=>x.ToDto());
            return Ok(lessonDtos);
        }
    }
}