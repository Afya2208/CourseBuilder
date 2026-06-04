using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class UserHasCourse
{
    public int CourseId { get; set; }

    public long UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
