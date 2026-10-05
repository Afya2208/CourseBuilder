using Domain;

namespace API.Features.Feedbacks;

public interface IFeedbackService
{
    Task AddFeedbackAsync(FeedbackSubmitDto submitDto);
    Task SolveFeedbackAsync(int feedbackId);
    Task<List<FeedbackCategoryDto>> FindAllCategoriesAsync();
    Task<FeedbacksDtoList> PagingSearchAsync(int pageSize, int pageNumber, bool solved = false);
    Task<int> CountUnsolvedAsync();
}

public class FeedbackService(IFeedbackRepository feedbackRepository) : IFeedbackService
{
    public async Task AddFeedbackAsync(FeedbackSubmitDto submitDto)
    {
        await feedbackRepository.AddAsync(submitDto.MapToEntity());
    }

    public async Task SolveFeedbackAsync(int feedbackId)
    {
        await feedbackRepository.SolveAsync(feedbackId);
    }

    public async Task<List<FeedbackCategoryDto>> FindAllCategoriesAsync()
    {
        return await feedbackRepository.ReadAllCategoriesAsync();
    }

    public async Task<FeedbacksDtoList> PagingSearchAsync(int pageSize, int pageNumber, bool solved = false)
    {
        return await feedbackRepository.PagingSearchAsync(pageSize, pageNumber, solved);
    }

    public async Task<int> CountUnsolvedAsync()
    {
        return await feedbackRepository.CountUnsolvedAsync();
    }
}