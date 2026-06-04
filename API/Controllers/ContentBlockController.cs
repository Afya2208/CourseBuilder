using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class ContentBlockController(ContentBlockRepository contentBlockRepository,
    ContentBlockTypeRepository contentBlockTypeRepository, CoursesDbContext context,
    IWebHostEnvironment environment) : ControllerBase
    {
        [HttpGet("lessons/{lessonId:long}/content-blocks")]
        public async Task<IActionResult> FindByLessonId(long lessonId)
        {
            var contents = await context.ContentBlocks
                .Where(x=>x.LessonId == lessonId)
                .OrderBy(x=>x.Order)
                .Select(x=> new
                {
                    x.Id, x.Name, x.ContentBlockTypeId,
                    x.LessonId, x.TextValue, x.FileNameView, x.Order
                })
                .ToListAsync();
            return Ok(contents);
        }
        [HttpGet("content-block-types")]
        public async Task<IActionResult> FindAllTypes()
        {
            var types = await context.ContentBlockTypes
                .Select(x=> new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();
            return Ok(types);
        }

        [HttpPost("content-blocks")]
        public async Task<IActionResult> Add([FromForm] ContentBlockFormData contentBlock)
        {
            var newBlock = new ContentBlock();
            context.Entry(newBlock).CurrentValues.SetValues(contentBlock);
            if (contentBlock.FormFile != null && contentBlock.FormFile.Length > 0 && !string.IsNullOrWhiteSpace(contentBlock.FormFile.FileName))
            {
                var newFileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(contentBlock.FormFile.FileName);
                var fileName = Path.GetFileName(contentBlock.FormFile.FileName);
                var path = Path.Combine(environment.ContentRootPath, "ContentFiles");
                    
                var newDirName = newFileName.Substring(0, 1);
                var newDir = Directory.CreateDirectory(Path.Combine(path, newDirName)).FullName;
                    
                await using var stream = new FileStream(Path.Combine(newDir, newFileName + extension), FileMode.Create);
                await contentBlock.FormFile.CopyToAsync(stream);
                    
                newBlock.FileNameView = fileName;
                newBlock.FileNameStorage = newFileName + extension;
            } 
            context.ContentBlocks.Add(newBlock);
            await context.SaveChangesAsync();
            return Ok(newBlock.Id);
        }

        [HttpPut("content-blocks")]
        public async Task<IActionResult> Update([FromForm] ContentBlockFormData contentBlock)
        {
            var oldBlock = await context.ContentBlocks.FindAsync(contentBlock.Id);
            if (oldBlock != null)
            {
                var oldType = oldBlock.ContentBlockTypeId;
                var oldFileNameStorage = oldBlock.FileNameStorage;
                context.Entry(oldBlock).CurrentValues.SetValues(contentBlock);
                var path = Path.Combine(environment.ContentRootPath, "ContentFiles");
                if (contentBlock.FormFile != null && contentBlock.FormFile.Length > 0 && !string.IsNullOrWhiteSpace(contentBlock.FormFile.FileName))
                {
                    var newFileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(contentBlock.FormFile.FileName);
                    var fileName = Path.GetFileName(contentBlock.FormFile.FileName);
                    
                    var newDirName = newFileName.Substring(0, 1);
                    var newDir = Directory.CreateDirectory(Path.Combine(path, newDirName)).FullName;
                    
                    await using var stream = new FileStream(Path.Combine(newDir, newFileName + extension), FileMode.Create);
                    await contentBlock.FormFile.CopyToAsync(stream);
                    
                    oldBlock.FileNameView = fileName;
                    oldBlock.FileNameStorage = newFileName + extension;
                    // проверим, есть ли старый файл - если есть, то удаляем
                    if (oldFileNameStorage != null)
                    {
                        var oldDir = oldFileNameStorage.Substring(0, 1);
                        if (System.IO.File.Exists(Path.Combine(path, oldDir, oldFileNameStorage)))
                        {
                            System.IO.File.Delete(Path.Combine(path, oldDir, oldFileNameStorage));
                        }
                    }
                }
                // если ранее был файл - а потом стал текстом, то удаляем старый файл
                if (oldType != contentBlock.ContentBlockTypeId && contentBlock.ContentBlockTypeId == 1)
                {
                    if (oldFileNameStorage != null)
                    {
                        var oldDir = oldFileNameStorage.Substring(0, 1);
                        if (System.IO.File.Exists(Path.Combine(path, oldDir, oldFileNameStorage)))
                        {
                            System.IO.File.Delete(Path.Combine(path, oldDir, oldFileNameStorage));
                        }
                    }
                    oldBlock.FileNameView = null;
                    oldBlock.FileNameStorage = null;
                }
                await context.SaveChangesAsync();
                return Ok();
            }
            throw new NotFoundException("Блок не найден");
        }

        [HttpDelete("content-blocks/{blockId:long}")]
        public async Task<IActionResult> Add(long blockId)
        {
            var block = await context.ContentBlocks.FindAsync(blockId);
            if (block == null) return NoContent();
            context.ContentBlocks.Remove(block);
            await context.SaveChangesAsync();
            if (block.FileNameStorage != null)
            {
                var path = Path.Combine(environment.ContentRootPath, "ContentFiles");
                var oldDir = block.FileNameStorage.Substring(0, 1);
                if (System.IO.File.Exists(Path.Combine(path, oldDir, block.FileNameStorage)))
                {
                    System.IO.File.Delete(Path.Combine(path, oldDir, block.FileNameStorage));
                }
            }
            return Ok(block);
        }
    }
}