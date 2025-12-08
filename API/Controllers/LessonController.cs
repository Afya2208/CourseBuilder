using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class LessonController(LessonRepository lessonRepository) : ControllerBase
    {
        [HttpGet("modules/{moduleId:long}/lessons")]
        public async Task<IActionResult> FindAllByModuleId(long moduleId)
        {
            List<Lesson> foundLessons = await lessonRepository.FindAllByConditionAsync(x => x.ModuleId == moduleId);
            return Ok(foundLessons);
        }
    }
}