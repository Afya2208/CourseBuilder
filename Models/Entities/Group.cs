using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Group
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public long CuratorId { get; set; }

    public string? CuratorFeedback { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateOnly DateStart { get; set; }

    public DateOnly DateEnd { get; set; }

    public int MaxMembersCount { get; set; }

    public virtual ICollection<Course>? Courses { get; set; } = new List<Course>();

    public virtual User? Curator { get; set; } = null!;

    public virtual ICollection<UserInGroup>? UserInGroups { get; set; } = new List<UserInGroup>();

    public virtual ICollection<Course>? CoursesNavigation { get; set; } = new List<Course>();
}
