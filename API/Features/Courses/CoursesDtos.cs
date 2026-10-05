using API.Features.Themes;
using Domain.Entities;

namespace API.Features.Courses;

public record CourseDto(
    int Id,
    string Name,
    string? Description,
    decimal? Price,
    long? AuthorId,
    int? LinkedGroupId,
    bool IsPublic,
    bool ModulesHaveOrder,
    List<ThemeDto> Themes,
    int? ModulesCount,
    int? LessonsCount)
{
    public CourseDto(Course c) : this(c.Id,c.Name,c.Description,c.Price,
        c.AuthorId,c.LinkedGroupId,c.IsPublic,c.ModulesHaveOrder,
        c.Themes.Select(t=>new ThemeDto(t.Id,t.Name)).ToList(), c.Modules.Count, 
        c.Modules.Sum(m=>m.Lessons.Count))
    {
        
    }
}

public record AddUpdateCourseRequest(int Id, string Name, string? Description,
    decimal? Price, long? AuthorId, bool ModulesHaveOrder, List<int>? ThemesIds,
    int MinimalCompletionPercentage)
{
    public Course MapToEntity()
    {
        return new Course()
        {
            Id = Id, Name = Name, Description = Description,
            Price = Price ?? 0, AuthorId = AuthorId, ModulesHaveOrder = ModulesHaveOrder,
            
        };
    }
}

public record CoursesDtoList(List<CourseDto> Courses, int TotalCount);

public record CourseShortDto(int Id, string Name);

public static class CoursesDtos
{
    public static IQueryable<CourseDto> SelectDto(this IQueryable<Course> query)
    {
        return query.Select(c => new CourseDto(c.Id, c.Name, c.Description,
            c.Price, c.AuthorId, c.LinkedGroupId, c.IsPublic, c.ModulesHaveOrder, 
            c.Themes.Select(t=>new ThemeDto(t.Id, t.Name)).ToList(), c.Modules.Count, 
            c.Modules.Sum(m=>m.Lessons.Count)));
    }
    public static IQueryable<CourseShortDto> SelectShortDto(this IQueryable<Course> query)
    {
        return query.Select(c => new CourseShortDto(c.Id, c.Name));
    }
}




