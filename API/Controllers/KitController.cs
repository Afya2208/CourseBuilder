using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    [Route("kits")]
    public class KitController(CoursesDbContext context) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> AddKit([FromBody] KitDto kit)
        {
            var newKit = new Kit();
            context.Entry(newKit).CurrentValues.SetValues(kit);

            var coursesIds = kit.CoursesInfo.Select(x=>x.Id);
            newKit.Courses = await context.Courses.Where(x=>coursesIds.Contains(x.Id)).ToListAsync();
            await context.Kits.AddAsync(newKit);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateKit([FromBody] KitDto kit)
        {
            var oldKit = await context.Kits.Include(x => x.Courses).FirstOrDefaultAsync(x => x.Id == kit.Id);
            context.Entry(oldKit).CurrentValues.SetValues(kit);
            oldKit.Courses.Clear();
            var coursesIds = kit.CoursesInfo.Select(x => x.Id);
            oldKit.Courses = await context.Courses.Where(x => coursesIds.Contains(x.Id)).ToListAsync();
            await context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpDelete("{kitId:int}")]
        public async Task<IActionResult> Delete(int kitId)
        {
            var kit = await context.Kits.FindAsync(kitId);
            if (kit != null)
            {
                context.Kits.Remove(kit);
                await context.SaveChangesAsync();
                return Ok();
            }
            return NoContent();
        }

        [HttpPost("/users/{userId:long}/add-kit/{kitId:int}")]
        [Authorize]
        public async Task<IActionResult> AddKitToUser(long userId, int kitId)
        {
            await context.UserHasKits.AddAsync(new UserHasKit()
            {
                UserId = userId,
                KitId = kitId,
            });
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("available-for/{userId:long}")]
        public async Task<IActionResult> GetUserKits(long userId)
        {
            var data = context.UserHasKits.AsNoTracking().Where(x => x.UserId == userId);
            var kits = await data
            .Select(x => new
            {
                Id = x.Kit.Id,
                Name = x.Kit.Name,
                Description = x.Kit.Description,
                AuthorId = x.Kit.AuthorId,
                CoursesInfo = x.Kit.Courses.Select(c => new { c.Id, c.Name })
            })
            .ToListAsync();
            return Ok(kits);
        }
        [HttpGet("available-for/{userId:long}/ids")]
        public async Task<IActionResult> GetUserKitsIds(long userId)
        {
            var data = context.UserHasKits.AsNoTracking().Where(x => x.UserId == userId);
            var kitsIds = await data
            .Select(x => x.KitId)
            .ToListAsync();
            return Ok(kitsIds);
        }

        [HttpGet("{kitId:int}")]
        public async Task<IActionResult> GetUserKits(int kitId)
        {
            var kit = await context.Kits.Where(x => x.Id == kitId)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Description,
                x.Price,
x.AuthorId,
                CoursesInfo = x.Courses.Select(c => new { c.Id, c.Name })
            })
            .FirstOrDefaultAsync();
            return Ok(kit);
        }
        
        [HttpGet("by-user/{userId:long}")]
        public async Task<IActionResult> KitsByUser(long userId)
        {
            var kits = await context.Kits.Where(x => x.AuthorId == userId)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Description,
                x.Price,
                x.AuthorId,
                CoursesInfo = x.Courses.Select(c => new { c.Id, c.Name })
            })
            .ToListAsync();
            return Ok(kits);
        }
        
        [HttpGet("search")]
        public async Task<IActionResult> PagingSearch(int pageSize, string? text, int? themeId, int pageNumber)
        {
            var query = context.Kits.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(x => EF.Functions.ILike(x.Name + " " + x.Description, $"%{text}%"));
            }
            if (themeId != null)
            {
                query = query.Where(x => x.Courses.Any(c=>c.Themes.Any(t => t.Id == themeId)));
            }
            var totalCount = await query.CountAsync();
            var kits = await query
            .OrderBy(x=>x.Id)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Price,
                x.AuthorId,
                x.Description,
                CoursesInfo = x.Courses.Select(c=> new {c.Id, c.Name})
            })
            .ToListAsync();
            return Ok(new
            {
                totalCount,
                kits
            });
        }
    }
}