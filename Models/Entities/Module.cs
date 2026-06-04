using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Module
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool LessonsHaveOrder { get; set; }

    public int CourseId { get; set; }

    public int Order { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
