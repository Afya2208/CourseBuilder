using System.Linq.Expressions;
using Domain.Entities;

namespace API.Features.Kits;

public record KitDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    long AuthorId,
    List<CourseInfoForKitDto> Courses)
{
    public KitDto(Kit k) : this(k.Id, k.Name, k.Description, k.Price,
        k.AuthorId, k.Courses.Select(c => new CourseInfoForKitDto(c.Id, c.Name)).ToList())
    {
        
    }
}

public record KitShortDto(int Id, string Name, string Description, long AuthorId, List<CourseInfoForKitDto> Courses);

public record CourseInfoForKitDto(int Id, string Name);

public record KitsDtoList(List<KitDto> Kits, int TotalCount);

public static class KitsDtos
{
    public static IQueryable<KitDto> SelectDto(this IQueryable<Kit> query)
    {
        return query.Select(SelectToDto);
    }
    
    public static readonly Expression<Func<Kit, KitDto>> SelectToDto =
        k => new KitDto(k.Id, k.Name,  k.Description, k.Price, k.AuthorId,
            k.Courses.Select(c=> new CourseInfoForKitDto(c.Id, c.Name)).ToList());
}