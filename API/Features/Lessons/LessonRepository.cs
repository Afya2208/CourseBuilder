using API.Util;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Lessons;

public interface ILessonRepository
{
    Task<LessonDto?> ReadByIdAsync(long lessonId);
    Task<bool> IsLessonDoneByUserAsync(long lessonId, long userId);
    Task<NextOrPrevLessonLink> GetPreviousLessonIdAsync(long lessonId);
    Task<NextOrPrevLessonLink> GetNextLessonIdAsync(long lessonId);
    Task<List<LessonDto>> ReadAllByModuleIdAsync(long moduleId);
    Task SaveLessonsOrderAsync(IEnumerable<LessonOrderDto> lessonOrders);
    Task DeleteAsync(long lessonId);
    Task<Lesson> UpdateAsync(long lessonId, Lesson lesson);
    Task<Lesson> AddAsync(Lesson lesson);
    Task<List<LessonInfoForReportDto>> ReadCourseLessonsInfoForReportAsync(int courseId);
}

public class LessonRepository(CoursesDbContext context) : ILessonRepository
{
    public async Task<int> GetRequiredLessonsCountForCourse(int courseId)
    {
        return context.Lessons.Where(x=>x.Module.CourseId == courseId && x.IsRequired).Include(x=>x.Module).Count();
    }

    public async Task<LessonDto?> ReadByIdAsync(long lessonId)
    {
        return await context.Lessons
            .AsNoTracking()
            .Where(x=>x.Id == lessonId)
            .SelectDto()
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsLessonDoneByUserAsync(long lessonId, long userId)
    {
        return await context.UserTrySolveTasks
            .AnyAsync(x=> x.AllSolved && x.LessonId == lessonId && x.UserId == userId);
    }

    public async Task<NextOrPrevLessonLink> GetPreviousLessonIdAsync(long lessonId)
    {
        return await context.GetPreviousLessonIdAsync(lessonId);
    }
    
    public async Task<NextOrPrevLessonLink> GetNextLessonIdAsync(long lessonId)
    {
        return await context.GetNextLessonIdAsync(lessonId);
    }

    public async Task<List<LessonDto>> ReadAllByModuleIdAsync(long moduleId)
    {
        return await context.Lessons
            .AsNoTracking()
            .Where(l => l.ModuleId == moduleId)
            .OrderBy(l => l.Order)
            .SelectDto()
            .ToListAsync();
    }

    public async Task SaveLessonsOrderAsync(IEnumerable<LessonOrderDto> lessonOrders)
    {
        var dictOrder = lessonOrders.ToDictionary(l => l.Id);
        var lessons = await context.Lessons.Where(l => dictOrder.Keys.Contains(l.Id)).ToListAsync();
        foreach (var lesson in lessons)
        {
            lesson.Order = dictOrder[lesson.Id].Order;
        }
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long lessonId)
    {
        var lesson = await context.Lessons.FindAsync(lessonId);
        if (lesson == null)
            throw new NotFoundException("Не найдено занятие", lessonId);
        context.Lessons.Remove(lesson);
        await context.SaveChangesAsync();
    }

    public async Task<Lesson> UpdateAsync(long lessonId, Lesson lessonData)
    {
        var oldLesson = await context.Lessons.FindAsync(lessonId);
        if (oldLesson == null)
            throw new NotFoundException("Не найдено занятие", lessonId);
        context.Entry(oldLesson).CurrentValues.SetValues(lessonData);
        await context.SaveChangesAsync();
        return oldLesson;
    }

    public async Task<Lesson> AddAsync(Lesson lesson)
    {
        var saved = await context.Lessons.AddAsync(lesson);
        await context.SaveChangesAsync();
        return saved.Entity;
    }

    public async Task<List<LessonInfoForReportDto>> ReadCourseLessonsInfoForReportAsync(int courseId)
    {
        return await context.Lessons
            .AsNoTracking()
            .Where(x => x.Module.CourseId == courseId)
            .OrderBy(x => x.Module.Order)
            .ThenBy(x => x.Order)
            .Select(x => new LessonInfoForReportDto(x.Id,
                x.Name, x.Tasks.Sum(t => t.Score), x.Module.Order, x.Order, x.Module.Name))
            .ToListAsync();
    }
}
