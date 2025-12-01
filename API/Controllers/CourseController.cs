using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    [Route("courses")]
    public class CourseController(CourseRepository courseRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await courseRepository.FindAllAsync([x=>x.Themes]));
        }
        [HttpGet("{courseId:int}")]
        public async Task<IActionResult> FindById(int courseId)
        {
            return Ok(await courseRepository.FindByIdAsync(courseId, [x=>x.Themes]));
        }
          [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] Course course)
        {
            return Ok(await courseRepository.AddAsync(course));
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] Course course)
        {
             return Ok(await courseRepository.UpdateAsync(course));
        }
        [HttpDelete("{courseId:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int courseId)
        {
             return Ok(await courseRepository.DeleteAsync(courseId));
        }
    }
}