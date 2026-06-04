using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class FeedbackSubmit
{
    public int Id { get; set; }

    public long? UserId { get; set; }

    public string? Email { get; set; }

    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;

    public int FeedbackCategoryId { get; set; }

    public DateTime DateTimeSent { get; set; }

    public DateTime? DateTimeSolved { get; set; }

    public virtual FeedbackCategory FeedbackCategory { get; set; } = null!;

    public virtual User? User { get; set; }
}
