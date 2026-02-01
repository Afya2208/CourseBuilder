using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class TaskAnswer
{
    public long Id { get; set; }

    public string? TextValue { get; set; }

    public byte[]? File { get; set; }

    public string? FileName { get; set; }

    public long TaskId { get; set; }

    public bool? IsRight { get; set; }

    public long? UserId { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User? User { get; set; }
}
