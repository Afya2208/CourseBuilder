using Domain.Entities;

namespace API.Features.Feedbacks;

public record FeedbackCategoryDto(int Id, string Name);

public record FeedbacksDtoList(List<FeedbackSubmitDto> FeedbackSubmits, int TotalCount);

public record FeedbackSubmitDto(
    int Id,
    long? UserId,
    string? Email,
    string Title,
    string Text,
    DateTime DateTimeSent,
    DateTime? DateTimeSolved,
    int FeedbackCategoryId)
{
    public FeedbackSubmitDto(FeedbackSubmit f) : this(f.Id, f.UserId,
        f.Email, f.Title, f.Text, f.DateTimeSent, f.DateTimeSolved, f.FeedbackCategoryId)
    {

    }

    public FeedbackSubmit MapToEntity()
    {
        return new FeedbackSubmit()
        {
            Id = Id, UserId = UserId, Email = Email, 
            Title = Title, Text = Text, DateTimeSent = DateTimeSent, 
            DateTimeSolved = DateTimeSolved, FeedbackCategoryId = FeedbackCategoryId
        };
    }
}


public static class FeedbacksDtos
{
    public static IQueryable<FeedbackSubmitDto> SelectDto(this IQueryable<FeedbackSubmit> query)
    {
        return query.Select(f => new FeedbackSubmitDto(f.Id, f.UserId,
            f.Email, f.Title, f.Text, f.DateTimeSent, f.DateTimeSolved, f.FeedbackCategoryId));
    }
}
