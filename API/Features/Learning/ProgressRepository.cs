using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Learning;

public interface IProgressRepository
{
    Task<List<UserTryInfoDto>> ReadUsersTriesInLessonsForReportAsync(IEnumerable<long> usersIds, IEnumerable<long> lessonsIds);
    Task<UserTrySolveTask> SaveUserTryLessonAsync(long userId, long lessonId, List<UserTrySolveDetail> details);
    Task<IsCourseDoneByUser> CheckIfCourseIsNeededToStateDoneByUserAsync(long userId, long lessonId);
    Task SaveUserCompletedCourseAsync(long userId, int courseId);
    Task AddAdditionalTryToUserAsync(long userId, long lessonId);
    Task<CourseProgressDto> GetCourseProgressForUserAsync(long userId, int courseId);
    Task<int?> GetUnusedTriesAsync(long userId, long lessonId);
    Task<bool> CheckIsLessonDoneByUserAsync(long userId, long lessonId);
}

public class ProgressRepository(CoursesDbContext context) : IProgressRepository
{
    public async Task<List<UserTryInfoDto>> ReadUsersTriesInLessonsForReportAsync(IEnumerable<long> usersIds, IEnumerable<long> lessonsIds)
    {
       return await context.UserTrySolveTasks
            .Where(x => usersIds.Contains(x.UserId) && lessonsIds.Contains(x.LessonId))
            .OrderByDescending(x => x.DateTime)
            .Select(x => new { x.UserId, x.LessonId, 
                    SumScore = x.UserTrySolveDetails.Where(d => d.IsSolved).Sum(d => d.Task.Score) })
            .GroupBy(x => new { x.UserId, x.LessonId })
            .Select(g => new UserTryInfoDto(g.Key.UserId, g.Key.LessonId, g.Max(vals => vals.SumScore)))
            .ToListAsync();
    }

    public async Task<UserTrySolveTask> SaveUserTryLessonAsync(long userId, long lessonId, List<UserTrySolveDetail> details)
    {
        var newTry = new UserTrySolveTask()
        {
            UserId = userId,
            LessonId = lessonId,
            AllSolved = details.All(x=>x.IsSolved),
            UserTrySolveDetails = details
        };
        var saved = await context.UserTrySolveTasks.AddAsync(newTry);
        await context.SaveChangesAsync();
        return saved.Entity;
    }

    public async Task<IsCourseDoneByUser> CheckIfCourseIsNeededToStateDoneByUserAsync(long userId, long lessonId)
    {
        var courseId = await context.Lessons.Where(x => x.Id == lessonId).Select(x => x.Module.CourseId).FirstOrDefaultAsync();
        if (courseId == 0)
            throw new NotFoundException("Не найден курс по занятию", lessonId);
        if (await context.UserHasDoneCourses.AnyAsync(x=>x.UserId == userId && x.CourseId == courseId))
        {
            return new IsCourseDoneByUser(courseId, false);
        }
        bool isDone = await context.Courses.AnyAsync(c => c.Id == courseId && c.Modules.All(m =>
            m.Lessons.All(l => l.IsRequired && l.UserTrySolveTasks.Any(t => t.UserId == userId && t.AllSolved))));
        return new IsCourseDoneByUser(courseId, isDone);
    }

    public async Task SaveUserCompletedCourseAsync(long userId, int courseId)
    {
        await context.UserHasDoneCourses.AddAsync(new UserHasDoneCourse()
        {
            CourseId = courseId,
            UserId = userId
        });
        await context.SaveChangesAsync();
    }

    public async Task AddAdditionalTryToUserAsync(long userId, long lessonId)
    {
        await context.AdditionalTryForUsers.AddAsync(new AdditionalTryForUser()
        {
            UserId = userId, LessonId = lessonId
        });
        await context.SaveChangesAsync();
    }

    public async Task<CourseProgressDto> GetCourseProgressForUserAsync(long userId, int courseId)
    {
        var progressData = await context.Lessons
            .Where(l => l.Module.CourseId == courseId && l.IsRequired && l.Tasks.Any())
            .Select(l => new
            {
                l.Id,
                IsSolved = l.UserTrySolveTasks.Any(u => u.UserId == userId && u.AllSolved)
            })
            .ToListAsync();

        var totalCount = progressData.Count;
        var solvedCount = progressData.Count(x => x.IsSolved);
        var progressPercent = totalCount == 0 ? 100 : Math.Round((double)solvedCount / totalCount * 100, 1);
        var isComplete = solvedCount == totalCount;

        return new CourseProgressDto(progressPercent, solvedCount, totalCount, isComplete,
            totalCount == 0 ? "NoRequiredTasks" : isComplete ? "Completed" : "InProgress");
    }

    public async Task<int?> GetUnusedTriesAsync(long userId, long lessonId)
    {
        int? triesCount = null;
        var lessonMaxCountTries = await context.Lessons.Where(l => l.Id == lessonId).Select(l => l.MaxTriesCount).FirstOrDefaultAsync() ?? 0;
        if (lessonMaxCountTries != 0)
        {
            var userTries = await context.UserTrySolveTasks.CountAsync(x => x.LessonId == lessonId && x.UserId == userId);
            if (triesCount < lessonMaxCountTries)
            {
                triesCount = lessonMaxCountTries - userTries;
            }
            else
            {
                var unusedTriesCount = await context.AdditionalTryForUsers.CountAsync(x => x.LessonId == lessonId && x.UserId == userId && !x.IsUsed);
                triesCount = unusedTriesCount;
            }
        }
        return triesCount;
    }

    public async Task<bool> CheckIsLessonDoneByUserAsync(long userId, long lessonId)
    {
        return await context.UserTrySolveTasks.AnyAsync(x=>x.AllSolved && x.LessonId == lessonId && x.UserId == userId);
    }
}