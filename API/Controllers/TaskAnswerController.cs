using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class TaskAnswerController(TaskAnswerRepository taskAnswerRepository) : ControllerBase
    {
        [HttpGet("tasks/{taskId:long}/answers")]
        public async Task<IActionResult> FindAllByTaskId(long taskId)
        {
            var answers = await taskAnswerRepository.FindAllByConditionAsync(x => x.TaskId == taskId);
            var answersDto = answers.ConvertAll(x=>x.ToDto());
            return Ok(answersDto);
        }
        [Authorize]
        [HttpPost("tasks/{taskId:long}/save-answer")]
        public async Task<IActionResult> SaveAnswer([FromBody] TaskAnswerDto taskAnswerDto)
        {
            return Ok(await taskAnswerRepository.AddAsync(taskAnswerDto.ToEntity()));
        }
    }
}