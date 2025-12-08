using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]

    public class ContentBlockController(ContentBlockRepository contentBlockRepository) : ControllerBase
    {
        [HttpGet("lessons/{lessonId:long}/content-blocks")]
        public async Task<IActionResult> FindByLessonId(long lessonId)
        {
            return Ok(await contentBlockRepository.FindAllByConditionAsync(x=>x.LessonId == lessonId));
        }
    }
}