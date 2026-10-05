namespace Domain.Entities;

public class NextOrPrevLessonLink
{
    public long ModuleId { get; set; }
    public long? LessonId { get; set; }
}