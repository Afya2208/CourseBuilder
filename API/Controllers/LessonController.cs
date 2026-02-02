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
    public class LessonController(LessonRepository lessonRepository, LessonTypeRepository lessonTypeRepository) : ControllerBase
    {

        [HttpGet("lessons/{lessonId:long}")]
        public async Task<IActionResult> FindById(long lessonId)
        {
            Lesson? lesson = await lessonRepository.FindByIdAsync(lessonId);
            if (lesson == null) throw new NotFoundException($"Не найдено занятие id={lessonId}");
            return Ok(lesson.ToDto());
        }

        [HttpGet("lesson-types")]
        public async Task<IActionResult> FindAllLessonTypes()
        {
            var types = await lessonTypeRepository.FindAllAsync();
            return Ok(types);
        }

        [HttpGet("modules/{moduleId:long}/lessons")]
        public async Task<IActionResult> FindAllByModuleId(long moduleId)
        {
            List<Lesson> foundLessons = await lessonRepository.FindAllByConditionAsync(x => x.ModuleId == moduleId);
            List<LessonDto> lessonDtos = foundLessons.ConvertAll(x => x.ToDto());
            return Ok(lessonDtos);
        }
        
        [HttpPost("lessons")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> AddLesson([FromBody] LessonDto lessonToAdd)
        {
            return Ok((await lessonRepository.AddAsync(lessonToAdd.ToEntity())).ToDto());
        }
        [HttpPut("lessons")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Update([FromBody] LessonDto lessonToUpdate)
        {
             return Ok((await lessonRepository.UpdateAsync(lessonToUpdate.ToEntity())).ToDto());
        }
        [HttpDelete("lessons/{lessonId:long}")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Delete(long lessonId)
        {
            var deletedLesson = await lessonRepository.DeleteAsync(lessonId);
            if (deletedLesson != null) return Ok(deletedLesson.ToDto());
            return NoContent();
        }
    }
}