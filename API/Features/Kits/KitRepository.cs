using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Kits;

public interface IKitRepository
{
    Task<Kit> AddAsync(KitDto kitDto);
    Task<KitsDtoList> PagingSearchAsync(int pageSize, int pageNumber, string? text, int? themeId);
    Task<Kit> UpdateAsync(int kitId, KitDto kitDto);
    Task DeleteAsync(int kitId);
    Task<KitDto?> ReadById(int kitId);
    Task<List<KitDto>> FindAllKitsByAuthorId(long authorId);
    Task<List<KitDto>> ReadAllKitsByUserIdAsync(long userId);
    Task AddKitToUserAsync(long userId, int kitId);
    Task<List<KitShortDto>> ReadAllKitsUserHasAsync(long userId);
    Task<List<int>> ReadAllKitsIdUserHasAsync(long userId);
}

public class KitRepository(CoursesDbContext context) : IKitRepository
{
    public async Task<Kit> AddAsync(KitDto kitDto)
    {
        var newKit = new Kit();
        context.Entry(newKit).CurrentValues.SetValues(kitDto);
        var coursesIds = kitDto.Courses.Select(x=>x.Id);
        newKit.Courses = await context.Courses.Where(x=>coursesIds.Contains(x.Id)).ToListAsync();
        var added = await context.Kits.AddAsync(newKit);
        var savedKit = added.Entity;
        await context.SaveChangesAsync();
        return savedKit;
    }
    
    public async Task<KitsDtoList> PagingSearchAsync(int pageSize, int pageNumber, string? text, int? themeId)
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
            .SelectDto()
            .ToListAsync();
        return new KitsDtoList(kits, totalCount);
    }

    public async Task<Kit> UpdateAsync(int kitId, KitDto kitDto)
    {
        var oldKit = await context.Kits
            .Include(x => x.Courses)
            .FirstOrDefaultAsync(x => x.Id == kitId);
        if (oldKit == null)
            throw new NotFoundException("Не найден набор курсов", kitId);
        context.Entry(oldKit).CurrentValues.SetValues(kitDto);
        oldKit.Courses.Clear();
        var coursesIds = kitDto.Courses.Select(x => x.Id);
        oldKit.Courses = await context.Courses.Where(x => coursesIds.Contains(x.Id)).ToListAsync();
        await context.SaveChangesAsync();
        return oldKit;
    }

    public async Task DeleteAsync(int kitId)
    {
        var kit = await context.Kits.FindAsync(kitId);
        if (kit == null)
            throw new NotFoundException("Не найден набор курсов", kitId);
        context.Kits.Remove(kit);
        await context.SaveChangesAsync();
    }

    public Task<KitDto?> ReadById(int kitId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<KitDto>> FindAllKitsByAuthorId(long authorId)
    {
        return await context.Kits.AsNoTracking()
            .Where(x => x.AuthorId == authorId)
            .SelectDto()
            .ToListAsync();
    }

    public async Task<List<KitDto>> ReadAllKitsByUserIdAsync(long userId)
    {
        return await context.Kits
            .AsNoTracking()
            .Where(k => k.AuthorId == userId)
            .SelectDto()
            .ToListAsync();
    }

    public async Task AddKitToUserAsync(long userId, int kitId)
    {
        await context.UserHasKits.AddAsync(new UserHasKit()
        {
            KitId = kitId,
            UserId = userId
        });
        await context.SaveChangesAsync();
    }

    public async Task<List<KitShortDto>> ReadAllKitsUserHasAsync(long userId)
    {
        return await context.UserHasKits
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(uhc => new KitShortDto(uhc.KitId, uhc.Kit.Name, uhc.Kit.Description, uhc.Kit.AuthorId,
                uhc.Kit.Courses.Select(c => new CourseInfoForKitDto(c.Id, c.Name)).ToList()))
            .ToListAsync();
            
    }

    public async Task<List<int>> ReadAllKitsIdUserHasAsync(long userId)
    {
        return await context.UserHasKits
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(uhc => uhc.KitId)
            .ToListAsync();
    }
}