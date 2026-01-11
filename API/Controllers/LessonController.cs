using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class LessonController(LessonRepository lessonRepository,
        TaskRepository taskRepository, ContentBlockRepository contentBlockRepository,
        TaskAnswerRepository taskAnswerRepository, CorrelationRepository correlationRepository) : ControllerBase
    {

        [HttpGet("lessons/{lessonId:long}")]
        public async Task<IActionResult> FindById(long lessonId)
        {
            Lesson? lesson = await lessonRepository.FindByIdAsync(lessonId);
            if (lesson == null) throw new NotFoundException($"Не найдено занятие id={lessonId}");
            return Ok(lesson);
        }
        [HttpGet("modules/{moduleId:long}/lessons")]
        public async Task<IActionResult> FindAllByModuleId(long moduleId)
        {
            List<Lesson> foundLessons = await lessonRepository.FindAllByConditionAsync(x => x.ModuleId == moduleId);
            return Ok(foundLessons);
        }

         // 2. Получение полного списка содержания занятия
        [HttpGet("lessons/{lessonId:long}/full")]
        public async Task<IActionResult> FindByLessonId(long lessonId)
        {  
            // задания и ответы к ним, нужно работать с типами заданий
            List<Models.Entities.Task> tasks = await taskRepository.FindAllByConditionAsync(x=>x.LessonId == lessonId);
            tasks.ForEach(x=>
            {
                switch (x.TaskTypeId)
                {
                    case 1: // текстовое задание с проверкой, то есть есть 1 answer
                    
                    break;
                }
            });
            // содержательные блоки
            // сортировка данных по порядку
            return Ok();
        }
    }
}