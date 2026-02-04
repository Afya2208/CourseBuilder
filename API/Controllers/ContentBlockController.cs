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

        [HttpPost("content-blocks")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Add([FromBody] ContentBlock contentBlock)
        {
            var saved = await contentBlockRepository.AddAsync(contentBlock);
            return Ok(saved);
        }
        [HttpPost("content-blocks/with-file")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> AddWithFile([FromForm] ContentBlock contentBlock)
        {
            /*
            var newBlock = new ContentBlock
            {
                ContentBlockTypeId = dto.ContentBlockTypeId,
                Order = dto.Order,
                Name = dto.Name,
                TextValue = dto.TextValue,
                LessonId = dto.LessonId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Обработка файла, если он есть
            if (dto.File != null && dto.File.Length > 0)
            {
                await ProcessFileUpload(dto.File, contentBlock);
            }
            */
            var saved = await contentBlockRepository.AddAsync(contentBlock);
            return Ok(saved);
        }

        [HttpPut("content-blocks")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Update(ContentBlock contentBlock)
        {
            var saved = await contentBlockRepository.UpdateAsync(contentBlock);
            return Ok(saved);
        }
        [HttpPut("content-blocks/with-file")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> UpdateWithFile([FromForm]ContentBlock contentBlock)
        {
            var saved = await contentBlockRepository.UpdateAsync(contentBlock);
            return Ok(saved);
        }

        [HttpDelete("content-blocks/{blockId:long}")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Add(long blockId)
        {
            var deleted = await contentBlockRepository.DeleteAsync(blockId);
            if (deleted == null) return NoContent();
            return Ok(deleted);
        }
    }
}