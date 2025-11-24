using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class UserInformation
{
    public long UserId { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? Phone { get; set; }

    public string? Position { get; set; }

    public virtual User User { get; set; } = null!;
}
