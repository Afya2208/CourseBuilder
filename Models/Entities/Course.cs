using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public long? AuthorId { get; set; }

    public bool ModulesHaveOrder { get; set; }

    public int MinimalCompletionPercentage { get; set; }

    public virtual User? Author { get; set; }

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<Course> ForCourses { get; set; } = new List<Course>();

    public virtual ICollection<Course> NeededCourses { get; set; } = new List<Course>();

    public virtual ICollection<Theme> Themes { get; set; } = new List<Theme>();
}
