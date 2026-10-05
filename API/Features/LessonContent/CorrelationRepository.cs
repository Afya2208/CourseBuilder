using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Features.LessonContent
{
    public interface ICorrelationRepository
    {
        Task<List<CorrelationDto>> ReadAllCorrelationsOfTaskAsync(long taskId);
    }
    public class CorrelationRepository(CoursesDbContext context) : ICorrelationRepository
    {
        public async Task<List<CorrelationDto>> ReadAllCorrelationsOfTaskAsync(long taskId)
        {
            return await context.Correlations
                .AsNoTracking()
                .Where(c => c.TaskId == taskId)
                .SelectDto()
                .ToListAsync();
        }
    }
}