using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace API.Features.LessonContent
{
    public interface ITaskAnswerRepository
    {
        Task SaveUserAnswerAsync(TaskAnswer taskAnswer);
        Task<List<TaskAnswerDto>> ReadAllTaskAnswersAsync(long taskId);
    }
    public class TaskAnswerRepository(CoursesDbContext context) : ITaskAnswerRepository
    {
        public async Task SaveUserAnswerAsync(TaskAnswer taskAnswer)
        {
            await context.TaskAnswers.AddAsync(taskAnswer);
            await context.SaveChangesAsync();
        }

        public async Task<List<TaskAnswerDto>> ReadAllTaskAnswersAsync(long taskId)
        {
            return await context.TaskAnswers
                .AsNoTracking()
                .Where(t => t.TaskId == taskId)
                .Select(t => new TaskAnswerDto(t.Id, t.TextValue, t.TaskId, t.UserId, t.IsRight))
                .ToListAsync();
        }
    }
}