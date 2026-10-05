using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.LessonContent
{
    [ApiController]
    [Route("tasks")]
    [Authorize("AllUsers")]
    public class TaskAnswerController(ITaskAnswerRepository taskAnswerRepository) : ControllerBase
    {
        [HttpGet("{taskId:long}/answers")]
        public async Task<ActionResult<List<TaskAnswerDto>>> FindAllByTaskId(long taskId)
        {
            return Ok(await taskAnswerRepository.ReadAllTaskAnswersAsync(taskId));
        }
        
        [HttpPost("{taskId:long}/user-answer")]
        public async Task<ActionResult> SaveAnswer([FromBody] TaskAnswerDto taskAnswerDto)
        {
            await taskAnswerRepository.SaveUserAnswerAsync(taskAnswerDto.MapToEntity());
            return NoContent();
        }
    }
}