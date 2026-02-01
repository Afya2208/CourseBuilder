using API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace API.Controllers;
[ApiController]
public class FileController(CoursesDbContext context) : ControllerBase
{
    [HttpGet("content-blocks/{contentBlockId:long}/file")]
    public async Task<IActionResult> FindFileById(long contentBlockId)
    {
        var file = await context.ContentBlocks.FirstOrDefaultAsync(e => e.Id == contentBlockId);
        if (file == null || file.File == null) throw new NotFoundException($"Файл не найден");
        var mimeType = file.FileName.Split('.').Last() switch
        {
            "png" => "image/png",
            "jpg" => "image/jpeg",
            "jpeg" => "image/jpeg",
            "gif" => "image/gif",
            "pdf" => "application/pdf",
            _=> "application/octet-stream"
        };
        return File(file.File, mimeType, file.FileName);
    }
    [HttpGet("content-blocks/{contentBlockId:long}/file-stream")]
    public async Task<IActionResult> FindFileStreamById(long contentBlockId)
    {
        var file = await context.ContentBlocks.FirstOrDefaultAsync(e => e.Id == contentBlockId);
        if (file == null || file.File == null) throw new NotFoundException($"Файл не найден");
        return File(file.File, "application/octet-stream", file.FileName);
    }
}