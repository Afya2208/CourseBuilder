using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("course")]
    public class CourseController(CourseRepository courseRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await courseRepository.FindAllAsync());
        }
    }
}