using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;

namespace API.Controllers
{
    [ApiController]
    public class CorrelationController(CorrelationRepository correlationRepository) : ControllerBase
    {
        [HttpGet("tasks/{taskId:long}/correlations")]
        public async Task<IActionResult> FindAllByTaskId(long taskId)
        {
            var corrs = await correlationRepository.FindAllByConditionAsync(x => x.TaskId == taskId);
            var corrsDto = corrs.ConvertAll(x=>x.ToDto());
            return Ok(corrsDto);
        }
    }
}