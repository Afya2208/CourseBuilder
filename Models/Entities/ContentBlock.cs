using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class ContentBlock
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public int ContentBlockTypeId { get; set; }

    public string? TextValue { get; set; }

    public byte[]? File { get; set; }

    public string? FileName { get; set; }

    public long LessonId { get; set; }

    public int Order { get; set; }

    public virtual ContentBlockType ContentBlockType { get; set; } = null!;

    public virtual Lesson Lesson { get; set; } = null!;
}
