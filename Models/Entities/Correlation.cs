using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Correlation
{
    public long Id { get; set; }

    public string Left { get; set; } = null!;

    public string Right { get; set; } = null!;

    public long TaskId { get; set; }

    public virtual Task Task { get; set; } = null!;
}
