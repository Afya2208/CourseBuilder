using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [Route("progress")]
    [ApiController]
    public class ProgressController(LessonRepository lessonRepository,
        CoursesDbContext context, ILogger<ProgressController> logger) : Controller
    {
        [HttpPost("{userId:long}/tried-lesson/{lessonId:long}")]
        public async Task<IActionResult> UserTriedLessonTasks(long userId, long lessonId, [FromBody] List<UserTrySolveDetail> details)
        {
            var newTry = new UserTrySolveTask()
            {
                UserId = userId,
                LessonId = lessonId,
                AllSolved = details.All(x=>x.IsSolved),
                UserTrySolveDetails = details
            };
            await context.UserTrySolveTasks.AddAsync(newTry);
            await context.SaveChangesAsync();
            try
            {
                if (newTry.AllSolved)
                {
                    var courseId = await context.Lessons
                        .Where(x => x.Id == lessonId)
                        .Select(x => x.Module.CourseId)
                        .FirstOrDefaultAsync();
                    if (courseId != null)
                    {
                        var mandatoryLessonIds = await context.Lessons
                            .Where(l => l.Module.CourseId == courseId && l.IsRequired && l.Tasks.Any())
                            .Select(l => l.Id)
                            .ToListAsync();
                        var solvedMandatoryCount = await context.UserTrySolveTasks
                            .Where(x => x.UserId == userId 
                                        && x.AllSolved 
                                        && mandatoryLessonIds.Contains(x.LessonId)) 
                            .Select(x => x.LessonId)
                            .Distinct()
                            .CountAsync();
                        if (mandatoryLessonIds.Count() == solvedMandatoryCount)
                        {
                            var alreadyCompleted = await context.UserHasDoneCourses
                                .AnyAsync(x => x.CourseId == courseId && x.UserId == userId);

                            if (!alreadyCompleted)
                            {
                                await context.UserHasDoneCourses.AddAsync(new UserHasDoneCourse()
                                {
                                    CourseId = courseId,
                                    UserId = userId
                                });
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.InnerException?.Message);
            }
            return Ok();
        }

        [HttpGet("for-group/{groupId:int}/course/{courseId:int}")]
        public async Task<IActionResult> GroupProgress(int groupId, int courseId)
        {
            var users = await context.UserInGroups
            .Where(x => x.GroupId == groupId && x.JoinStatusId == 1)
            .Select(x => new InfoAboutUser()
            {
                Id = x.UserId,
                LastName = x.User.UserInformation.LastName,
                FirstName = x.User.UserInformation.FirstName
            })
            .OrderBy(x => x.LastName)
            .ToListAsync();
            var lessons = await context.Lessons
            .Where(x => x.Module.CourseId == courseId)
            .Select(x => new LessonInfo()
            {
                Id = x.Id,
                Name = x.Name,
                MaxScore = x.Tasks.Sum(t => t.Score),
                ModuleOrder = x.Module.Order,
                LessonOrder = x.Order,
                ModuleName = x.Module.Name
            })
            .OrderBy(x => x.ModuleOrder)
            .ThenBy(x => x.LessonOrder)
            .ToListAsync();

            var usersIds = users.Select(x => x.Id);
            var lessonsIds = lessons.Select(x => x.Id);


            var tries = await context.UserTrySolveTasks
            .Include(x => x.UserTrySolveDetails).ThenInclude(x => x.Task)
            .Where(x =>
                usersIds.Contains(x.UserId) &&
                lessonsIds.Contains(x.LessonId)
            )
            .ToListAsync();

            var latestTries = tries
            .GroupBy(x => new { x.UserId, x.LessonId })
            .Select(g => g.OrderByDescending(x => x.DateTime).First())

            .Select(x => new UserTriesInfo()
            {
                UserId = x.UserId,
                LessonId = x.LessonId,
                SumScore = x.UserTrySolveDetails.Where(t => t.IsSolved).Sum(o => o.Task?.Score ?? 0)
            })
            .ToList();

            var courseName = (await context.Courses.FindAsync(courseId)).Name;
            var groupName = (await context.Groups.FindAsync(groupId)).Name;

            var bytes = await ProgressDocumentor.XlsxGroupProgressForCourse(groupName, courseName, users, lessons, latestTries);

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Успеваемость {groupName} {courseName}.xlsx");
        }
        
        [HttpGet("for-user/{userId:long}/course/{courseId:int}")]
        public async Task<IActionResult> ProgressForUser(long userId, int courseId)
        {
            var lessons = await context.Lessons
            .Where(x => x.Module.CourseId == courseId)
            .Select(x => new LessonInfo()
            {
                Id = x.Id,
                Name = x.Name,
                MaxScore = x.Tasks.Sum(t => t.Score),
                ModuleOrder = x.Module.Order,
                LessonOrder = x.Order,
                ModuleName = x.Module.Name
            })
            .OrderBy(x=>x.ModuleOrder)
            .ThenBy(x=>x.LessonOrder)
            .ToListAsync();

            var lessonsIds = lessons.Select(x => x.Id);

            var latestTries = await context.UserTrySolveTasks
            .Where(x => x.UserId == userId && lessonsIds.Contains(x.LessonId))
            .GroupBy(x => x.LessonId)
            .Select(g => g.OrderByDescending(x => x.DateTime).First())
            .Select(x=> new 
            {
                LessonId = x.LessonId,
                SumScore = x.UserTrySolveDetails.Where(t=>t.IsSolved).Sum(o => o.Task.Score)
            })
            .ToListAsync();

            return Ok(new
            {
                lessons, tries = latestTries
            });
        }

        [HttpGet("{userId:long}/for-module/{moduleId:long}")]
        public async Task<IActionResult> ModuleProgress(long userId, long moduleId)
        {
            var lessonsSolved = await context.UserTrySolveTasks
            .Where(x => x.UserId == userId && x.Lesson.ModuleId == moduleId)
            .Select(x => new
            {
                x.LessonId,
                x.DateTime,
                SumScore = x.UserTrySolveDetails.Where(d => d.IsSolved).Sum(d => d.Task.Score),
                MaxScore = x.Lesson.Tasks.Sum(t => t.Score)
            })
            .OrderByDescending(x => x.DateTime)
            .ToListAsync();
            return Ok(lessonsSolved);
        }
        
        [HttpGet("unused-tries-for/{userId:long}/lesson/{lessonId:long}")]
        public async Task<IActionResult> LeftTries(long userId, long lessonId)
        {
            int? maxCount = (await context.Lessons.FindAsync(lessonId)).MaxTriesCount;
            if (maxCount == null)
            {
                return NoContent();
            }
            else
            {
                var triesCount = await context.UserTrySolveTasks.CountAsync(x => x.LessonId == lessonId && x.UserId == userId);
                if (triesCount < maxCount)
                {
                    return Ok(maxCount - triesCount);
                }
                else
                {
                    var unusedTriesCount = await context.AdditionalTryForUsers.CountAsync(x => x.LessonId == lessonId && x.UserId == userId
                    && !x.IsUsed);
                    return Ok(unusedTriesCount);
                }
            }
        }
        
        [HttpGet("{userId:long}/course/{courseId:int}")]
        public async Task<IActionResult> Progress(long userId, int courseId)
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

            return Ok(new
            {
                progressPercent,
                solvedCount,
                totalCount,
                isComplete,
                status = totalCount == 0 ? "NoRequiredTasks" : isComplete ? "Completed" : "InProgress"
            });
        }
        
        [HttpPost("add-try-for/{userId:long}/lesson/{lessonId:long}")]
        public async Task<IActionResult> AddTryForUser(long userId, long lessonId)
        {
            var newTry = new AdditionalTryForUser()
            {
                UserId = userId,
                LessonId = lessonId,
            };
            await context.AdditionalTryForUsers.AddAsync(newTry);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}