using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class Task
{
    public long Id { get; set; }

    public string Question { get; set; } = null!;

    public int TaskTypeId { get; set; }

    public long LessonId { get; set; }

    public int Order { get; set; }

    public virtual ICollection<Correlation> Correlations { get; set; } = new List<Correlation>();

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<TaskAnswer> TaskAnswers { get; set; } = new List<TaskAnswer>();

    public virtual TaskType TaskType { get; set; } = null!;
}
