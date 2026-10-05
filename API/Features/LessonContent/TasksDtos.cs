using Domain.Entities;
using Task = Domain.Entities.Task;

namespace API.Features.LessonContent;


public record TaskDto(long Id, string Question, int TaskTypeId, long LessonId, int Order, int Score)
{
    public TaskDto(Task t) : this(t.Id, t.Question, t.TaskTypeId, t.LessonId, t.Order, t.Score)
    {
        
    }
}

public record TaskTypeDto(int Id, string Name);

public record TaskAnswerDto(long Id, string? TextValue, long TaskId, long? UserId, bool? IsRight)
{
    public TaskAnswer MapToEntity()
    {
        return new TaskAnswer()
        {
            UserId = UserId,
            TextValue = TextValue,
            Id = Id,
            TaskId = TaskId,
            IsRight = IsRight
        };
    }
}

public record CorrelationDto(long Id, string Left, string Right, long? TaskId);

public record TaskOrderDto(long Id, int Order);

public class TaskAddUpdateRequest
{
    public long Id { get; set; }

    public string? Question { get; set; }
    public string? TextAnswer { get; set; }
    public List<CorrelationDto>? Correlations { get; set; } = new();
    public List<TaskAnswerDto>? AllAnswerOptions { get; set; } = new();

    public int TaskTypeId { get; set; }

    public long LessonId { get; set; }

    public int Order { get; set; }
    public int Score { get; set; }

}


public static class TasksDtos
{
    public static IQueryable<CorrelationDto> SelectDto(this IQueryable<Correlation> query)
    {
        return query.Select(x => new CorrelationDto(x.Id, x.Left, x.Right, x.TaskId));
    }
}