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
    public class ContentBlockController(ContentBlockRepository contentBlockRepository,
    ContentBlockTypeRepository contentBlockTypeRepository) : ControllerBase
    {
        [HttpGet("lessons/{lessonId:long}/content-blocks")]
        public async Task<IActionResult> FindByLessonId(long lessonId)
        {
            var contentBlocks = await contentBlockRepository.FindAllByLessonId(lessonId);
            return Ok(contentBlocks);
        }

        [HttpGet("content-block-types")]
        public async Task<IActionResult> FindAllTypes()
        {
            var contentBlocks = await contentBlockTypeRepository.FindAllAsync();
            return Ok(contentBlocks);
        }
    }
}