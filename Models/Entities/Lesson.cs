using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Lesson
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int LessonTypeId { get; set; }

    public long ModuleId { get; set; }

    public int Order { get; set; }

    public bool IsRequired { get; set; }

    public DateOnly? ClosedUntil { get; set; }

    public int? MaxTriesCount { get; set; }

    public virtual ICollection<AdditionalTryForUser> AdditionalTryForUsers { get; set; } = new List<AdditionalTryForUser>();

    public virtual ICollection<ContentBlock> ContentBlocks { get; set; } = new List<ContentBlock>();

    public virtual LessonType LessonType { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<UserTrySolveTask> UserTrySolveTasks { get; set; } = new List<UserTrySolveTask>();
}
