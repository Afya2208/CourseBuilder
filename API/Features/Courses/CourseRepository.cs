using API.Util;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Courses
{
    public interface ICourseRepository
    {
        Task<CourseDto?> ReadByIdAsync(int courseId);
        Task<CoursesDtoList> PagingSearchAsync(int pageSize, int pageNumber, string? text, int? themeId);
        Task ChangePublicityAsync(int courseId);
        Task<Course> AddAsync(Course courseToAdd, List<int> requestThemesIds);
        Task<Course> UpdateAsync(int courseId, Course courseData, List<int> requestThemesIds);
        Task DeleteAsync(int courseId);
        Task<List<CourseDto>> ReadAllCoursesByUserIdAsync(long userId);
        Task<List<CourseShortDto>> ReadAllCoursesShortByUserIdAsync(long userId);
        Task<List<CourseDto>> ReadAllCoursesUserHasAsync(long userId);
        Task AddCourseToUserAsync(long userId, int courseId);
        Task<List<int>> ReadAllCoursesIdUserHasAsync(long userId);
        Task<List<CourseShortDto>> ReadAllCoursesShortUserHasAsync(long userId);
        Task<string> ReadCourseNameByIdAsync(int courseId);
        Task<bool> CheckIsUserCourseAuthorAsync(long userId, int courseId);
    }
    public class CourseRepository(CoursesDbContext context) : ICourseRepository
    {
        public async Task ChangePublicityAsync(int courseId)
        {
            var course = await context.Courses.FindAsync(courseId);
            if (course == null)
                throw new NotFoundException("Не найден курс", courseId);
            course.IsPublic = !course.IsPublic;
            await context.SaveChangesAsync();
        }

        public async Task<Course> AddAsync(Course courseToAdd, List<int> requestThemesIds)
        {
            var added = await context.Courses.AddAsync(courseToAdd);
            var savedCourse = added.Entity;
            if (requestThemesIds.Any())
            {
                savedCourse.Themes = await context.Themes
                    .Where(t=> requestThemesIds.Contains(t.Id)).ToListAsync();
            }
            await context.SaveChangesAsync();
            return savedCourse;
        }

        public async Task<Course> UpdateAsync(int courseId, Course courseData, List<int> requestThemesIds)
        {
            var oldCourse = await context.Courses
                .Include(x=>x.Themes)
                .FirstOrDefaultAsync(x=>x.Id == courseId);
            if (oldCourse == null)
                throw new NotFoundException("Не найден курс", courseId);
            context.Entry(oldCourse).CurrentValues.SetValues(courseData);
            if (requestThemesIds.Any())
            {
                oldCourse.Themes = await context.Themes
                    .Where(t=> requestThemesIds.Contains(t.Id)).ToListAsync();
            }
            await context.SaveChangesAsync();
            return oldCourse;
        }

        public async Task DeleteAsync(int courseId)
        {
            var course = await context.Courses.FindAsync(courseId);
            if (course == null)
                throw new NotFoundException("Не найден курс", courseId);
            context.Courses.Remove(course);
            await context.SaveChangesAsync();
        }

        public async Task<List<CourseDto>> ReadAllCoursesByUserIdAsync(long userId)
        {
            return await context.Courses
                .AsNoTracking()
                .Where(c => c.AuthorId == userId)
                .SelectDto()
                .ToListAsync();
        }

        public async Task<List<CourseShortDto>> ReadAllCoursesShortByUserIdAsync(long userId)
        {
            return await context.Courses
                .AsNoTracking()
                .Where(c => c.AuthorId == userId)
                .SelectShortDto()
                .ToListAsync();
        }

        public async Task<List<CourseDto>> ReadAllCoursesUserHasAsync(long userId)
        {
            var personal = context.UserHasCourses.Where(x => x.UserId == userId)
                .Select(x=>x.CourseId);
            var groupsLinkedCourses = context.UserInGroups.Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.Courses).Select(x=>x.Id);
            var groupsCourses = context.UserInGroups.Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.CoursesNavigation).Select(x=>x.Id);
            var kits = context.UserHasKits.Where(x => x.UserId == userId)
                .SelectMany(x=>x.Kit.Courses).Select(x=>x.Id);
            var unionCourseIds = personal.Union(groupsLinkedCourses).Union(groupsCourses).Union(kits);
            return await context.Courses
                .AsNoTracking()
                .Where(c => unionCourseIds.Contains(c.Id))
                .SelectDto()
                .ToListAsync();
        }

        public async Task AddCourseToUserAsync(long userId, int courseId)
        {
            await context.UserHasCourses.AddAsync(new UserHasCourse()
            {
                UserId = userId, CourseId = courseId
            });
            await context.SaveChangesAsync();
        }

        public async Task<List<int>> ReadAllCoursesIdUserHasAsync(long userId)
        {
            var userHasCoursesIdd = context.UserHasCourses.Where(x => x.UserId == userId
            ).Select(x=>x.CourseId);
            var createdByUserIds = context.Courses.Where(x => x.AuthorId == userId
            ).Select(x=>x.Id);
            var groups = context.UserInGroups.Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.Courses).Select(x=>x.Id);
            var groups2 = context.UserInGroups.Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.CoursesNavigation).Select(x=>x.Id);
            var kits = context.UserHasKits.Where(x => x.UserId == userId)
                .SelectMany(x=>x.Kit.Courses).Select(x=>x.Id);
            var query = userHasCoursesIdd.Union(createdByUserIds).Union(groups).Union(groups2).Union(kits);
            return await query.ToListAsync();
        }

        public async Task<List<CourseShortDto>> ReadAllCoursesShortUserHasAsync(long userId)
        {
            var userHasCoursesIdd = context.UserHasCourses.Where(x => x.UserId == userId).Select(x=>x.CourseId);
            var createdByUserIds = context.Courses.Where(x => x.AuthorId == userId).Select(x=>x.Id);
            var groups = context.UserInGroups.Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.Courses).Select(x=>x.Id);
            var groups2 = context.UserInGroups.Where(x => x.UserId == userId && x.JoinStatusId == 1)
                .SelectMany(x=>x.Group.CoursesNavigation).Select(x=>x.Id);
            var kits = context.UserHasKits.Where(x => x.UserId == userId)
                .SelectMany(x=>x.Kit.Courses).Select(x=>x.Id);
            var coursesIds = userHasCoursesIdd.Union(createdByUserIds).Union(groups).Union(groups2).Union(kits);
            return await context.Courses
                .AsNoTracking()
                .Where(c => coursesIds.Contains(c.Id))
                .SelectShortDto()
                .ToListAsync();
        }

        public async Task<string?> ReadCourseNameByIdAsync(int courseId)
        {
            return await context.Courses.Where(x=>x.Id == courseId).Select(x=>x.Name).FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIsUserCourseAuthorAsync(long userId, int courseId)
        {
            return await context.Courses.AnyAsync(x=>x.Id == courseId && x.AuthorId == userId);
        }

        public async Task<CourseDto?> ReadByIdAsync(int courseId)
        {
            return await context.Courses
                .AsNoTracking()
                .Where(x=> x.Id == courseId)
                .SelectDto()
                .FirstOrDefaultAsync();
        }

        public async Task<CoursesDtoList> PagingSearchAsync(int pageSize, int pageNumber, string? text, int? themeId)
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
            var courses = await query
                .OrderBy(x=>x.Id)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .SelectDto()
                .ToListAsync();
            return new CoursesDtoList(courses, totalCount);
        }
    }
}