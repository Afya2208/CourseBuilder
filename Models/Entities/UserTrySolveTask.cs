using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class UserTrySolveTask
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long LessonId { get; set; }

    public bool AllSolved { get; set; }

    public DateTime? DateTime { get; set; }

    public virtual Lesson? Lesson { get; set; } = null!;

    public virtual User? User { get; set; } = null!;

    public virtual ICollection<UserTrySolveDetail>? UserTrySolveDetails { get; set; } = new List<UserTrySolveDetail>();
}
