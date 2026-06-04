using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class UserHasKit
{
    public long UserId { get; set; }

    public int KitId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Kit Kit { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
