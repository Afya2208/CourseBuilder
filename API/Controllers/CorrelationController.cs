using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class CorrelationController(CorrelationRepository correlationRepository) : ControllerBase
    {
        [HttpGet("tasks/{taskId:long}/correlations")]
        public async Task<IActionResult> FindAllByTaskId(long taskId)
        {
            return Ok(await correlationRepository.FindAllByConditionAsync(x=>x.TaskId == taskId));
        }
    }
}