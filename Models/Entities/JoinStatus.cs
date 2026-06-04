using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class JoinStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserInGroup> UserInGroups { get; set; } = new List<UserInGroup>();
}
