using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class UserInGroup
{
    public long UserId { get; set; }

    public int GroupId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int JoinStatusId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual JoinStatus JoinStatus { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
