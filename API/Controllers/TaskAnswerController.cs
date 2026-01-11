using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]

    public class TaskAnswerController(TaskAnswerRepository taskAnswerRepository) : ControllerBase
    {
        [HttpGet("tasks/{taskId:long}/answers")]
        public async Task<IActionResult> FindAllByTaskId(long taskId)
        {
            return Ok(await taskAnswerRepository.FindAllByConditionAsync(x=>x.TaskId == taskId));
        }
        [HttpPost("task-answers")]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] TaskAnswer taskAnswerDto)
        {
            return Ok(await taskAnswerRepository.AddAsync(taskAnswerDto));
        }
        [HttpPut("task-answers")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] TaskAnswer taskAnswerDto)
        {
             return Ok(await taskAnswerRepository.UpdateAsync(taskAnswerDto));
        }
        [HttpDelete("task-answers/{themeId:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int taskAnswerId)
        {
             return Ok(await taskAnswerRepository.DeleteAsync(taskAnswerId));
        }
    }
}