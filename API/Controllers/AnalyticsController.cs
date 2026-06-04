using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace API.Controllers;

[ApiController]
[Route("analytics")]
public class AnalyticsController(CoursesDbContext context) : Controller
{
    // общая статистика по курсам для разработчика
    [HttpGet("courses/by-user/{userId:long}")]
    public async Task<IActionResult> CoursesByUser(long userId)
    {
        var usersCount = await context.UserHasCourses.CountAsync(x=>x.Course.AuthorId == userId);
        var individualIds = context.Courses.Where(x=>x.IsPublic);
        var usersAllSolved = await context.UserHasDoneCourses.CountAsync(x=>x.Course.AuthorId == userId);
        return Ok(new
        {
            totalUsersCount = usersCount,
            totalUsersDone = usersAllSolved,
        });
    }
    [HttpGet("courses/{courseId:long}")]
    public async Task<IActionResult> Course(int courseId)
    {
        var howManyBoughtIdn = await context.UserHasCourses.CountAsync(x=>x.CourseId == courseId); 
        var howManyBoughtKit = await context.UserHasKits.CountAsync(x=>x.Kit.Courses.Any(c=>c.Id == courseId)); 
        var howManyDone = await context.UserHasDoneCourses.CountAsync(x=>x.CourseId == courseId); 
        
        return Ok(new
        {
            individualCount = howManyBoughtIdn,
            fromKitsCount = howManyBoughtKit,
            doneCount = howManyDone,
        });
    }
    [HttpGet("groups/by-user/{userId:long}")]
    public async Task<IActionResult> GroupsByUser(long userId)
    {
        var usersGroup = await context.UserInGroups.CountAsync(x=>x.JoinStatusId == 2 && x.Group.CuratorId == userId);
        return Ok(new
        {
            totalUsersInGroupsCount = usersGroup,
        });
    }
    [HttpGet("kits/by-user/{userId:long}")]
    public async Task<IActionResult> KitsByUser(long userId)
    {
        var usersKit = await context.UserHasKits.CountAsync(x=>x.Kit.AuthorId == userId);
        
        return Ok();
    }
    // аналитика процента выполнения заданий - вычисляется сколько пользователей с этим курсом и сколько
    // процентов пользователей справились с занятием
    [HttpGet("percent-users-done-lessons/{courseId:int}")]
    public async Task<IActionResult> PercentUsersDoneLessons(int courseId)
    {
        var lessons = context.Lessons.Where(x=>x.Module.CourseId == courseId);
        var usersBlunt = await context.UserHasCourses.CountAsync(x=>x.CourseId == courseId);
        var usersKit = await context.UserHasKits.CountAsync(x=>x.Kit.Courses.Any(c=>c.Id == courseId));
        var usersGroup = await context.UserInGroups.CountAsync(x=>x.Group.Courses.Any(c=>c.Id == courseId));
        long totalUsersCount = usersGroup + usersBlunt + usersKit;
        var lessonsPercents = await lessons.Select(l => new
        {
            LessonId = l.Id,
            Percent = Math.Round(l.UserTrySolveTasks
                    .Where(t=>t.AllSolved && t.LessonId == l.Id)
                    .Select(t=>t.UserId)
                    .Distinct()
                    .Count() / (decimal)totalUsersCount * 100, 0)
        }).ToListAsync();
        return Ok(lessonsPercents);
    }
    // аналитика процента правильных попыток пройти задания - вычисляется сколько попыток решений и сколько полностью успешные
    [HttpGet("count-good-tries-for-lessons/{courseId:int}")]
    public async Task<IActionResult> PercentGoodTriesForLessons(int courseId)
    {
        var lessonsTries = await context.UserTrySolveTasks
            .Where(x=>x.Lesson.Module.CourseId == courseId)
            .GroupBy(x=> new {x.LessonId, x.AllSolved})
            .Select(x=> new
            {
                LessonId = x.Key.LessonId,
                AllSolved = x.Key.AllSolved,
                Count = x.Count(),
            }).ToListAsync();
        return Ok(lessonsTries);
    }
}