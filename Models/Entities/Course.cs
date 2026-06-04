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

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? LinkedGroupId { get; set; }

    public virtual User? Author { get; set; }

    public virtual Group? LinkedGroup { get; set; }

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<UserHasCourse> UserHasCourses { get; set; } = new List<UserHasCourse>();

    public virtual ICollection<UserHasDoneCourse> UserHasDoneCourses { get; set; } = new List<UserHasDoneCourse>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Kit> Kits { get; set; } = new List<Kit>();

    public virtual ICollection<Theme> Themes { get; set; } = new List<Theme>();
}
