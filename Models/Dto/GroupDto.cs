using Models.Entities;

namespace Models.Dto;

public class GroupDto
{
    public DateOnly DateStart { get; set; }

    public DateOnly DateEnd { get; set; }
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public long CuratorId { get; set; }
    public int MaxMembersCount { get; set; }
    public string? CuratorFeedback { get; set; }
    public virtual ICollection<Theme>? CoursesInfo { get; set; } = new List<Theme>();
}