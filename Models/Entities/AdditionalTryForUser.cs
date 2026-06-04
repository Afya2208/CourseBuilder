using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class AdditionalTryForUser
{
    public int Id { get; set; }

    public long UserId { get; set; }

    public long LessonId { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
