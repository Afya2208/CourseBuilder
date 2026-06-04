using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class LessonController(LessonRepository lessonRepository, LessonTypeRepository lessonTypeRepository,
        CoursesDbContext context) : ControllerBase
    {

        [HttpGet("lessons/{lessonId:long}")]
        public async Task<IActionResult> FindById(long lessonId)
        {
            Lesson? lesson = await lessonRepository.FindByIdAsync(lessonId);
            if (lesson == null) throw new NotFoundException($"Не найдено занятие id={lessonId}");
            return Ok(lesson.ToDto());
        }

        [HttpGet("/lessons/{lessonId:long}/done-by/{userId:long}")]
        public async Task<IActionResult> LessonDoneByUser(long lessonId, long userId)
        {
            var successTry = await context.UserTrySolveTasks.AnyAsync(x=>x.LessonId == lessonId
            && x.UserId == userId && x.AllSolved);
            return Ok(successTry);
        }
        [HttpGet("lesson-types")]
        public async Task<IActionResult> FindAllLessonTypes()
        {
            var types = await lessonTypeRepository.FindAllAsync();
            return Ok(types);
        }

        [HttpGet("courses/{courseId:int}/modules/{moduleId:long}/lessons/{lessonId:long}/next-lesson-id")]
        public async Task<IActionResult> NextLesson(int courseId, long moduleId, long lessonId)
        {
            var allLessons = await context.Lessons.Where(x => x.ModuleId == moduleId)
            .OrderBy(x => x.Order)
            .Select(x => new { x.Id, x.Order })
            .ToListAsync();
            var currentLessonIndex = allLessons.FindIndex(0, allLessons.Count, x => x.Id == lessonId);
            if (currentLessonIndex + 1 == allLessons.Count)
            {
                var modules = await context.Modules.Where(x => x.CourseId == courseId)
                .OrderBy(x => x.Order)
                .Select(x => new { x.Id, x.Order })
                .ToListAsync();
                var currentModuleIndex = modules.FindIndex(0, modules.Count, x => x.Id == moduleId);
                if (currentModuleIndex + 1 != modules.Count)
                {
                    
                    var nextModule = modules[currentModuleIndex + 1];
                    var firstLesson = await context.Lessons.Where(x => x.ModuleId == nextModule.Id)
                    .OrderBy(x => x.Order)
                    .FirstOrDefaultAsync();
                    if (firstLesson != null)
                    {
                        return Ok($"courses/{courseId}/modules/{nextModule.Id}/lessons/{firstLesson.Id}");
                    }
                }
            }
            else
            {
                var nextLesson = allLessons[currentLessonIndex + 1];
                return Ok($"courses/{courseId}/modules/{moduleId}/lessons/{nextLesson.Id}");
            }
            return NoContent();
        }
        [HttpGet("courses/{courseId:int}/modules/{moduleId:long}/lessons/{lessonId:long}/previous-lesson-id")]
        public async Task<IActionResult> PreviousLesson(int courseId, long moduleId, long lessonId)
        {
            var allLessons = await context.Lessons.Where(x => x.ModuleId == moduleId)
            .OrderBy(x => x.Order)
            .Select(x => new { x.Id, x.Order })
            .ToListAsync();
            var currentLessonIndex = allLessons.FindIndex(0, allLessons.Count, x => x.Id == lessonId);
            if (currentLessonIndex == 0)
            {
                var modules = await context.Modules.Where(x => x.CourseId == courseId)
                .OrderBy(x => x.Order)
                .Select(x => new { x.Id, x.Order })
                .ToListAsync();
                var currentModuleIndex = modules.FindIndex(0, modules.Count, x => x.Id == moduleId);
                if (currentModuleIndex != 0)
                {
                    var previousModule = modules[currentModuleIndex - 1];
                    var lastLesson = await context.Lessons.Where(x => x.ModuleId == previousModule.Id)
                    .OrderBy(x => x.Order)
                    .LastOrDefaultAsync();
                    if (lastLesson != null)
                    {
                        return Ok($"courses/{courseId}/modules/{previousModule.Id}/lessons/{lastLesson.Id}");
                    }
                }
            }
            else
            {
                var previousLesson = allLessons[currentLessonIndex - 1];
                return Ok($"courses/{courseId}/modules/{moduleId}/lessons/{previousLesson.Id}");
            }
            return NoContent();
        }

        [HttpGet("modules/{moduleId:long}/lessons")]
        public async Task<IActionResult> FindAllByModuleId(long moduleId)
        {
            var lessons = await context.Lessons
                .Where(x => x.ModuleId == moduleId)
                .OrderBy(x => x.Order)
                .ToListAsync();
            List<LessonDto> lessonDtos = lessons.ConvertAll(x => x.ToDto());
            return Ok(lessonDtos);
        }
        
        [HttpPost("/lessons/order")]
        public async Task<IActionResult> SaveLessonsOrder([FromBody] List<LessonOrder> lessonOrders)
        {
            foreach (var lessonOrder in lessonOrders)
            {
                var lesson = await context.Lessons.FindAsync(lessonOrder.Id);
                lesson.Order = lessonOrder.Order;
            }
            await context.SaveChangesAsync();
            return Ok();
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