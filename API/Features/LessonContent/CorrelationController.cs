using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.LessonContent
{
    [ApiController]
    [Authorize("AllUsers")]
    [Route("tasks")]
    public class CorrelationController(ICorrelationRepository correlationRepository) : ControllerBase
    {
        [HttpGet("{taskId:long}/correlations")]
        public async Task<ActionResult<CorrelationDto>> FindAllByTaskId(long taskId)
        {
            return Ok(await correlationRepository.ReadAllCorrelationsOfTaskAsync(taskId));
        }
    }
}