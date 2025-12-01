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
    [Route("task-answers")]
    public class TaskAnswerController(TaskAnswerRepository taskAnswerRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await taskAnswerRepository.FindAllAsync());
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] TaskAnswer taskAnswerDto)
        {
            return Ok(await taskAnswerRepository.AddAsync(taskAnswerDto));
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] TaskAnswer taskAnswerDto)
        {
             return Ok(await taskAnswerRepository.UpdateAsync(taskAnswerDto));
        }
        [HttpDelete("themeId:int")]
        [Authorize]
        public async Task<IActionResult> Delete(int taskAnswerId)
        {
             return Ok(await taskAnswerRepository.DeleteAsync(taskAnswerId));
        }
    }
}