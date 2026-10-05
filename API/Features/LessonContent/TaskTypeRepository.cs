using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Features.LessonContent
{
    public interface ITaskTypeRepository
    {
        Task<List<TaskTypeDto>> ReadAllTypesAsync();
    }
    public class TaskTypeRepository(CoursesDbContext context) : ITaskTypeRepository
    {
        public async Task<List<TaskTypeDto>> ReadAllTypesAsync()
        {
            return await context.TaskTypes.AsNoTracking().Select(x=> new TaskTypeDto(x.Id, x.Name)).ToListAsync();
        }
    }
}