using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.LessonContent
{
    [ApiController]
    [Route("tasks")]
    [Authorize("OnlyDev")]
    public class TaskController(ITaskRepository taskRepository, ITaskTypeRepository taskTypeRepository) : ControllerBase
    {
        [HttpGet("{taskId:long}")]
        public async Task<ActionResult<TaskDto>> FindById(long taskId)
        {
            return Ok(await taskRepository.ReadTaskByIdAsync(taskId));
        }
        
        [HttpGet("/task-types")]
        [Authorize("AllUsers")]
        public async Task<ActionResult<List<TaskTypeDto>>> FindAllTypes()
        {
            return Ok(await taskTypeRepository.ReadAllTypesAsync());
        }

        [HttpGet("/lessons/{lessonId:long}/tasks")]
        [Authorize("AllUsers")]
        public async Task<ActionResult<TaskDto>> FindByLessonId(long lessonId)
        {
            return Ok(await taskRepository.ReadAllLessonTasksAsync(lessonId));
        }
        
        [HttpPost("order")]
        public async Task<ActionResult> SaveTasksOrder([FromBody] IEnumerable<TaskOrderDto> taskOrders)
        {
            await taskRepository.SaveTasksOrderAsync(taskOrders);
            return NoContent();
        }

        [HttpDelete("{taskId:long}")]
        public async Task<ActionResult> Delete(long taskId)
        {
            await taskRepository.DeleteAsync(taskId);
            return NoContent();
        }
        
        [HttpPost]
        public async Task<ActionResult<TaskDto>> Add([FromBody] TaskAddUpdateRequest request)
        {
            var savedTask = await taskRepository.AddAsync(request);
            return Created(string.Empty, new TaskDto(savedTask));
        }
        
        [HttpPut("{taskId:long}")]
        public async Task<ActionResult<TaskDto>> Update([FromRoute] long taskId, [FromBody] TaskAddUpdateRequest request)
        {
            if (taskId != request.Id)
                throw new ArgumentException("Данные задачи не совпадают с переданным идентификатором");
            var savedTask = await taskRepository.UpdateAsync(taskId, request);
            return Created(string.Empty, new TaskDto(savedTask));
        }
    }
}