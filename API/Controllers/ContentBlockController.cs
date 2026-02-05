using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
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
        public async Task<IActionResult> Add([FromForm] ContentBlockFormData contentBlock)
        {
            var newBlock = new ContentBlock();
            newBlock.Name = contentBlock.Name;
            newBlock.TextValue = contentBlock.TextValue;
            newBlock.ContentBlockTypeId = contentBlock.ContentBlockTypeId;
            newBlock.FileName = contentBlock.FileName;
            newBlock.LessonId = contentBlock.LessonId;
            newBlock.Order = contentBlock.Order;
            if (contentBlock.File != null)
            {
                using var stream = new MemoryStream();
                await contentBlock.File.CopyToAsync(stream);
                newBlock.File = stream.ToArray(); 
                newBlock.FileName = contentBlock.FileName;
            }
            var saved = await contentBlockRepository.AddAsync(newBlock);
            return Ok(saved.Id);
        }

        [HttpPut("content-blocks")]
        public async Task<IActionResult> Update([FromForm] ContentBlockFormData contentBlock)
        {
            var oldBlock = await contentBlockRepository.FindByIdAsync(contentBlock.Id);
            if (oldBlock != null)
            {
                oldBlock.Name = contentBlock.Name;
                oldBlock.TextValue = contentBlock.TextValue;
                oldBlock.ContentBlockTypeId = contentBlock.ContentBlockTypeId;
                oldBlock.FileName = contentBlock.FileName;
                oldBlock.Order = contentBlock.Order;
                if (contentBlock.File != null)
                {
                    using var stream = new MemoryStream();
                    await contentBlock.File.CopyToAsync(stream);
                    oldBlock.File = stream.ToArray(); 
                    oldBlock.FileName = contentBlock.FileName;
                }
                var saved = await contentBlockRepository.UpdateAsync(oldBlock);
                return Ok();
            }
            throw new NotFoundException("Блок не найден");
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