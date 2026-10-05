using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Feedbacks;

public interface IFeedbackRepository
{
    Task AddAsync(FeedbackSubmit feedback);
    Task SolveAsync(int feedbackId);
    Task<List<FeedbackCategoryDto>> ReadAllCategoriesAsync();
    Task<FeedbacksDtoList> PagingSearchAsync(int pageSize, int pageNumber, bool solved = false);
    Task<int> CountUnsolvedAsync();
}

public class FeedbackRepository(CoursesDbContext context) : IFeedbackRepository
{
    public async Task AddAsync(FeedbackSubmit feedback)
    {
        await context.FeedbackSubmits.AddAsync(feedback);
        await context.SaveChangesAsync();
    }

    public async Task SolveAsync(int feedbackId)
    {
        var feedback = await context.FeedbackSubmits.FirstOrDefaultAsync(x => x.Id == feedbackId);
        if (feedback == null)
            throw new NotFoundException("Не найдено обращение", feedbackId);
        feedback.DateTimeSolved = DateTime.Now;
        await context.SaveChangesAsync();
    }

    public async Task<List<FeedbackCategoryDto>> ReadAllCategoriesAsync()
    {
        return await context.FeedbackCategories
            .AsNoTracking()
            .Select(c => new FeedbackCategoryDto(c.Id, c.Name))
            .ToListAsync();
    }

    public async Task<FeedbacksDtoList> PagingSearchAsync(int pageSize, int pageNumber, bool solved = false)
    {
        var query = context.FeedbackSubmits.AsNoTracking();
        if (solved)
        {
            query = query.Where(feedback => feedback.DateTimeSolved != null);
        }
        else
        {
            query = query.Where(feedback => feedback.DateTimeSolved == null);
        }
        var totalCount = await query.CountAsync();
        var list = await query
            .OrderByDescending(x => x.DateTimeSent)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .SelectDto()
            .ToListAsync();
        return new FeedbacksDtoList(list, totalCount);
    }

    public async Task<int> CountUnsolvedAsync()
    {
        return await context.FeedbackSubmits.CountAsync(feedback => feedback.DateTimeSolved == null);
    }
}