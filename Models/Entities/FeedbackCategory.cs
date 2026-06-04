using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class FeedbackCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<FeedbackSubmit> FeedbackSubmits { get; set; } = new List<FeedbackSubmit>();
}
