using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Kit
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public long AuthorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<UserHasKit> UserHasKits { get; set; } = new List<UserHasKit>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
