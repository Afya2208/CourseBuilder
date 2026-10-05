
using System.Text.Json.Serialization;
using Domain.Entities;

namespace API.Features.Modules;

public record ModuleDto(
    long Id,
    string Name,
    string? Description,
    int CourseId,
    bool LessonsHaveOrder,
    int Order,
    int? LessonsCount)
{

    public ModuleDto(Module m) : this(m.Id, m.Name, m.Description,
        m.CourseId, m.LessonsHaveOrder, m.Order, m.Lessons.Count)
    {
        
    }
    
    public Module MapToEntity()
    {
        return new Module()
        {
            Id = Id, Name = Name, Description = Description,
            CourseId = CourseId, LessonsHaveOrder = LessonsHaveOrder,
            Order = Order
        };
    }
}

public record ModuleOrderDto(long Id, int Order);

public static class ModulesDtos
{
    public static IQueryable<ModuleDto> SelectDto(this IQueryable<Module> query)
    {
        return query.Select(m => new ModuleDto(m.Id,
            m.Name, m.Description, m.CourseId, m.LessonsHaveOrder, m.Order,
            m.Lessons.Count));
    }
}