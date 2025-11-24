using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskController : ControllerBase
    {
        [HttpGet("{lessonId:long}")]
        public async Task<IActionResult> FindAllByLessonId(long lessonId)
        {
            return Ok();
        }
    }
}