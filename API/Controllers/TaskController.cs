using System;
using System.Collections.Generic;
using System.Linq;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class TaskController(TaskRepository taskRepository) : ControllerBase
    {
        // 1. Получение информации по 1 задаче по ее id
        [HttpGet("tasks/{taskId:long}")]
        public async Task<IActionResult> FindById(long taskId)
        {  
            Models.Entities.Task? found = await taskRepository.FindByIdAsync(taskId);
            if (found == null) throw new NotFoundException($"Не найдена задача id={taskId}");
            return Ok(found);
        }

        // 2. Получение полного списка заданий занятия
        [HttpGet("/lesson/{lessonId:long}/tasks")]
        public async Task<IActionResult> FindByLessonId(long lessonId)
        {  
            List<Models.Entities.Task> foundTasks = await taskRepository.FindAllByConditionAsync(x=>x.LessonId == lessonId);
            return Ok(foundTasks);
        }

        // 3. Удаление задачи
        [HttpDelete("tasks/{taskId:long}")]
        public async Task<IActionResult> Delete(long taskId)
        {  
            return Ok(await taskRepository.DeleteAsync(taskId));
        }
    }
}