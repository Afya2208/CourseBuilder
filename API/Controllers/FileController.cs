using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Exceptions;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;

namespace API.Controllers;
[ApiController]
public class FileController(CoursesDbContext context, IConfiguration configuration,
    IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet("content-blocks/{contentBlockId:long}/file")]
    public async Task<IActionResult> FindFileById(long contentBlockId)
    {
        var file = await context.ContentBlocks.FirstOrDefaultAsync(e => e.Id == contentBlockId);
        if (file == null || file.FileNameStorage == null || file.FileNameView == null) throw new NotFoundException($"Файл не найден");
        var mimeTyper = new FileExtensionContentTypeProvider();
        if (!mimeTyper.TryGetContentType(file.FileNameStorage, out var type))
        {
            type = "application/octet-stream";
        }
        var path = Path.Combine(environment.ContentRootPath, "ContentFiles");
        var dir = file.FileNameStorage.Substring(0, 1);
        if (System.IO.File.Exists(Path.Combine(path, dir, file.FileNameStorage)))
        {
            return PhysicalFile(Path.Combine(path, dir, file.FileNameStorage), type, file.FileNameView);
        }
        throw new NotFoundException($"Файл не найден");
    }
    [HttpGet("content-blocks/{contentBlockId:long}/file-download")]
    public async Task<IActionResult> FindFileStreamById(long contentBlockId)
    {
        var file = await context.ContentBlocks.FirstOrDefaultAsync(e => e.Id == contentBlockId);
        if (file == null || file.FileNameStorage == null || file.FileNameView == null) throw new NotFoundException($"Файл не найден");
        var path = Path.Combine(environment.ContentRootPath, "ContentFiles");
        var dir = file.FileNameStorage.Substring(0, 1);
        if (System.IO.File.Exists(Path.Combine(path, dir, file.FileNameStorage)))
        {
            return PhysicalFile(Path.Combine(path, dir, file.FileNameStorage), "application/octet-stream", file.FileNameView);
        }
        throw new NotFoundException($"Файл не найден");
    }
    
    [HttpGet("content-blocks/{contentBlockId:long}/video")]
    public async Task<IActionResult> FindVideo(long contentBlockId, [FromQuery] string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken)) return Unauthorized("No video token");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principal = handler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration["JWT:Issuer"], 
                ValidateLifetime = true,
                ValidAudience = configuration["JWT:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            }, out _);
            
            if (principal.FindFirstValue("VID") != contentBlockId.ToString())
                return Forbid("Токен не предназначен для этого видео");
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized("Ссылка истекла. Обновите страницу.");
        }
        catch
        {
            return Unauthorized("Недействительная подпись токена");
        }
        
        var videoBlock = await context.ContentBlocks.FirstOrDefaultAsync(e => e.Id == contentBlockId
            && e.ContentBlockTypeId == 5);
        if (videoBlock == null || videoBlock.FileNameStorage == null 
                               || videoBlock.FileNameView == null) throw new NotFoundException($"Файл не найден");
        var path = Path.Combine(environment.ContentRootPath, "ContentFiles");
        var dir = videoBlock.FileNameStorage.Substring(0, 1);
        if (System.IO.File.Exists(Path.Combine(path, dir, videoBlock.FileNameStorage)))
        {
            var fileStream = new FileStream(Path.Combine(path, dir, videoBlock.FileNameStorage), FileMode.Open,
                FileAccess.Read, FileShare.Read);
            var fileResult = new FileStreamResult(fileStream, "video/mp4")
            {
                EnableRangeProcessing = true
            };
            return fileResult;
        }
        throw new NotFoundException($"Файл не найден");
    }

    [HttpGet("token-for-video/{blockId:long}")]
   
    public async Task<IActionResult> TokenForVideo(long blockId)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null) return Unauthorized();
        var token = JwtTokens.GenerateVideoToken(configuration, userEmail, blockId);
        return Ok(new
        {
            url = $"{Request.Scheme}://{Request.Host}/content-blocks/{blockId}/video?accessToken={token}"
        });
    }
    
}