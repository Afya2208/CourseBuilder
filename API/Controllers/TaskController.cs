using System;
using System.Collections.Generic;
using System.Linq;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class TaskController(TaskRepository taskRepository, TaskTypeRepository taskTypeRepository) : ControllerBase
    {
        [HttpGet("tasks/{taskId:long}")]
        public async Task<IActionResult> FindById(long taskId)
        {
            Models.Entities.Task? found = await taskRepository.FindByIdAsync(taskId);
            if (found == null) throw new NotFoundException($"Не найдена задача id={taskId}");
            return Ok(found);
        }
        [HttpGet("task-types")]
        public async Task<IActionResult> FindAllTypes()
        {
            var types = await taskTypeRepository.FindAllAsync();
            var typesDto = types.ConvertAll(x=>x.ToDto());
            return Ok(typesDto);
        }

        [HttpGet("lessons/{lessonId:long}/tasks")]
        public async Task<IActionResult> FindByLessonId(long lessonId)
        {
            List<Models.Entities.Task> foundTasks = await taskRepository.FindAllByConditionAsync(x => x.LessonId == lessonId);
            var tasksDto = foundTasks.ConvertAll(x=>x.ToDto());
            return Ok(tasksDto);
        }

        [HttpDelete("tasks/{taskId:long}")]
        //[Authorize(Roles = "Разработчик")]
        public async Task<IActionResult> Delete(long taskId)
        {
            return Ok(await taskRepository.DeleteAsync(taskId));
        }
        [HttpPost("tasks")]
        //[Authorize(Roles = "Разработчик")]
        public async Task<IActionResult> Add([FromBody] TaskAddUpdateRequest request)
        {
            var tas = await taskRepository.AddAsync(request);
            return Ok(tas.ToDto());
        }
        
        [HttpPut("tasks")]
        //[Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Update([FromBody] TaskAddUpdateRequest request)
        {
            var tas = await taskRepository.UpdateAsync(request);
            return Ok(tas.ToDto());
        }
    }
}