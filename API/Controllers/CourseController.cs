using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    [Route("courses")]
    public class CourseController(CourseRepository courseRepository,
        ModuleRepository moduleRepository, LessonRepository lessonRepository, CoursesDbContext context) : ControllerBase
    {

        [HttpGet("{courseId:int}/is-created-by/{userId:long}")]
        public async Task<IActionResult> CourseIsCreatedByUser(long userId, int courseId)
        {
            var course = await courseRepository.FindByIdAsync(courseId);
            return Ok(course?.AuthorId == userId);
        }

        
        [HttpGet("available-for/{userId:long}/short")]
        public async Task<IActionResult> AvailableCoursesNames(long userId)
        {
            var personal = context.UserHasCourses.Where(x => x.UserId == userId)
                .Select(x=>x.CourseId);
            var groupsLinkedCourses = context.UserInGroups.Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.Courses).Select(x=>x.Id);
            var groupsCourses = context.UserInGroups.Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.CoursesNavigation).Select(x=>x.Id);
            var kits = context.UserHasKits.Where(x => x.UserId == userId)
                .SelectMany(x=>x.Kit.Courses).Select(x=>x.Id);
            var query = personal.Union(groupsLinkedCourses).Union(groupsCourses).Union(kits);
            var courses = await context.Courses.Where(x=>query.Contains(x.Id)).AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();
            return Ok(courses);
        }
        
        [HttpGet("available-for/{userId:long}")]
        public async Task<IActionResult> FindAllUserAvailableCourses(long userId)
        {
            var personal = context.UserHasCourses.Where(x => x.UserId == userId)
            .Select(x=>x.CourseId);
            var groupsLinkedCourses = context.UserInGroups.Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.Courses).Select(x=>x.Id);
            var groupsCourses = context.UserInGroups.Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.CoursesNavigation).Select(x=>x.Id);
            var kits = context.UserHasKits.Where(x => x.UserId == userId)
            .SelectMany(x=>x.Kit.Courses).Select(x=>x.Id);
            var query = personal.Union(groupsLinkedCourses).Union(groupsCourses).Union(kits);
            var courses = await context.Courses.Where(x=>query.Contains(x.Id)).AsNoTracking().Include(x=>x.Themes)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Price,
                x.LinkedGroupId,
                x.AuthorId,
                x.Description,
                ModulesCount = x.Modules.Count(),
                LessonsCount = x.Modules.Sum(t => t.Lessons.Count()),
                Themes = x.Themes.Select(x => x.ToDto()).ToList()
            })
            .ToListAsync();
            return Ok(courses);
        }

        [HttpGet("available-for/{userId:long}/ids")]
        public async Task<IActionResult> GetCourseIdsAvailableForUser(long userId)
        {
            var personal = context.UserHasCourses.Where(x => x.UserId == userId
            ).Select(x=>x.CourseId);
            var personal1 = context.Courses.Where(x => x.AuthorId == userId
            ).Select(x=>x.Id);
            var groups = context.UserInGroups.AsNoTracking().Where(x => x.UserId == userId && x.JoinStatusId == 1)
            .SelectMany(x=>x.Group.Courses).Select(x=>x.Id);
            var groups2 = context.UserInGroups.AsNoTracking().Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.CoursesNavigation).Select(x=>x.Id);
            var kits = context.UserHasKits.AsNoTracking().Where(x => x.UserId == userId)
            .SelectMany(x=>x.Kit.Courses).Select(x=>x.Id);
            var query = personal.Union(personal1).Union(groups).Union(groups2).Union(kits);
            var courses = await query.ToListAsync();
            return Ok(courses);
        }

        [HttpGet("by-user/{userId:long}")]
        public async Task<IActionResult> FindAllUserCreatedCourses(long userId)
        {
            var courses = await context.Courses.Where(x=>x.AuthorId == userId).AsNoTracking()
                .Include(x=>x.Themes)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Price,
                    x.LinkedGroupId,
                    x.AuthorId,
                    x.IsPublic,
                    x.Description,
                    ModulesCount = x.Modules.Count(),
                    LessonsCount = x.Modules.Sum(t => t.Lessons.Count()),
                    Themes = x.Themes.Select(x => x.ToDto()).ToList()
                })
                .ToListAsync();
            return Ok(courses);
        }
        [HttpGet("by-user/{userId:long}/short")]
        public async Task<IActionResult> Short(long userId)
        {
            var courses = await context.Courses.Where(x=>x.AuthorId == userId).AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                })
                .ToListAsync();
            return Ok(courses);
        }
        
        [HttpGet("{courseId:int}")]
        public async Task<IActionResult> FindById(int courseId)
        {
            Course? course = await courseRepository.FindByIdAsync(courseId, [x => x.Themes]);
            if (course == null) throw new NotFoundException($"Не найден курс id={courseId}");
            return Ok(course.ToDto());
        }

        [HttpGet("search")]
        public async Task<IActionResult> PagingSearch(int pageSize, string? text, int? themeId, int pageNumber)
        {
            var query = context.Courses.AsNoTracking().Where(x => x.IsPublic);
            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(x => EF.Functions.ILike(x.Name + " " + x.Description, $"%{text}%"));
            }
            if (themeId != null)
            {
                query = query.Where(x => x.Themes.Any(t => t.Id == themeId));
            }
            var totalCount = await query.CountAsync();
            var courses = await query.Include(x=>x.Themes)
            .OrderBy(x=>x.Id)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Price,
                x.LinkedGroupId,
                x.AuthorId,
                x.Description,
                ModulesCount = x.Modules.Count(),
                LessonsCount = x.Modules.Sum(t => t.Lessons.Count()),
                Themes = x.Themes.Select(x => x.ToDto()).ToList()
            })
            .ToListAsync();
            return Ok(new
            {
                totalCount,
                courses
            });
        }

        [HttpPost("{courseId:int}/change-publicity")]
        [Authorize]
        public async Task<IActionResult> ChangePublicity(int courseId)
        {
            await courseRepository.ChangePublicityAsync(courseId);
            return Ok();
        }

        [HttpPost("/users/{userId:long}/add-course/{courseId:int}")]
        [Authorize]
        public async Task<IActionResult> AddToUserAsync(long userId, int courseId)
        {
            await courseRepository.AddToUserAsync(userId, courseId);
            return Ok();
        }
        
        [HttpPost("/users/{userId:long}/add-course-group/{groupId:int}")]
        [Authorize]
        public async Task<IActionResult> AddUserToCourseGroup(long userId, int courseId, int groupId)
        {
            var group = await context.Groups.FindAsync(groupId);
            var nowCount = await context.UserInGroups.AsNoTracking().CountAsync(x => x.GroupId == groupId
            && x.JoinStatusId == 1);
            if (nowCount < group.MaxMembersCount)
            {
                await context.UserInGroups.AddAsync(new UserInGroup()
                {
                    UserId = userId,
                    GroupId = group.Id,
                    JoinStatusId = 1
                });
                await context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("К сожалению, набор в текущую группу по этому курсу уже закрыт");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Разработчик")]
        public async Task<IActionResult> Add([FromBody] CourseDto addCourseRequest)
        {
            var newCourse = new Course()
            {

            };
            context.Entry(newCourse).CurrentValues.SetValues(addCourseRequest);
            if (addCourseRequest.ThemesIds != null)
            {
                foreach (var themeId in addCourseRequest.ThemesIds)
                {
                    var theme = await context.Themes.FindAsync(themeId);
                    newCourse.Themes.Add(theme);
                }
            }
            await context.Courses.AddAsync(newCourse);
            await context.SaveChangesAsync();
            return Ok(newCourse);
        }

        [HttpPut]
        [Authorize(Roles = "Разработчик")]
        public async Task<IActionResult> Update([FromBody] CourseDto updateCourseRequest)
        {
            var oldCourse = await context.Courses
                .Include(x=>x.Themes)
                .FirstOrDefaultAsync(x => x.Id == updateCourseRequest.Id);
            oldCourse.Themes.Clear();
            context.Entry(oldCourse).CurrentValues.SetValues(updateCourseRequest);
            if (updateCourseRequest.ThemesIds != null)
            {
                foreach (var themeId in updateCourseRequest.ThemesIds)
                {
                    var theme = await context.Themes.FindAsync(themeId);
                    oldCourse.Themes.Add(theme);
                }
            }
            await context.SaveChangesAsync();
            return Ok(oldCourse);
        }
        
        [HttpDelete("{courseId:int}")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Delete(int courseId)
        {
            return Ok(await courseRepository.DeleteAsync(courseId));
        }
    }
}