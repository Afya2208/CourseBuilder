using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class User
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public int RoleId { get; set; }

    public byte[] Salt { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual Role Role { get; set; } = null!;

    public virtual UserInformation? UserInformation { get; set; }
}
