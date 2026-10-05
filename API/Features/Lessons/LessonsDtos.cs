using System.Text.Json.Serialization;
using Domain.Entities;

namespace API.Features.Lessons;


public record LessonOrderDto(long Id, int Order);

[method: JsonConstructor]
public record LessonDto(
    long Id,
    string Name,
    string? Description,
    bool IsRequired,
    int LessonTypeId,
    DateOnly? ClosedUntil,
    long ModuleId,
    int Order,
    int? MaxTriesCount)
{
    public LessonDto(Lesson l) : this(l.Id, l.Name, l.Description,
        l.IsRequired, l.LessonTypeId, l.ClosedUntil, l.ModuleId, l.Order, l.MaxTriesCount)
    {
        
    }

    public Lesson MapToEntity()
    {
        return new Lesson()
        {
            Id = Id, Name = Name, Description = Description,
            IsRequired = IsRequired, LessonTypeId = LessonTypeId,
            ModuleId = ModuleId, Order = Order, MaxTriesCount = MaxTriesCount,
            ClosedUntil = ClosedUntil
        };
    }
}


public record LessonInfoForReportDto(long Id, string Name, int MaxScore, int ModuleOrder,
    int LessonOrder, string ModuleName);
    

public record LessonTypeDto(int Id, string Name, string? Description);

public static class LessonsDtos
{
    public static IQueryable<LessonDto> SelectDto(this IQueryable<Lesson> query)
    {
        return query.Select(l=> new LessonDto(l.Id, l.Name, l.Description,
            l.IsRequired, l.LessonTypeId, l.ClosedUntil, l.ModuleId, l.Order, l.MaxTriesCount));
    }
}