using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class UserTrySolveDetail
{
    public long UserTryId { get; set; }

    public long TaskId { get; set; }

    public bool IsSolved { get; set; }

    public virtual Task? Task { get; set; } = null!;

    public virtual UserTrySolveTask? UserTry { get; set; } = null!;
}
