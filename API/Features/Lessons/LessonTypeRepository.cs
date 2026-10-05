using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace API.Features.Lessons;

public interface ILessonTypeRepository
{
    Task<IEnumerable<LessonTypeDto>> ReadAllAsync();
}

public class LessonTypeRepository(CoursesDbContext context) : ILessonTypeRepository
{
    public async Task<IEnumerable<LessonTypeDto>> ReadAllAsync()
    {
        return await context.LessonTypes.AsNoTracking().Select(t =>
            new LessonTypeDto(t.Id, t.Name, t.Description)).ToListAsync();
    }
}
