using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Models.Entities;

namespace API.Repositories
{
    public class CourseRepository(CoursesDbContext context) : BaseRepository<Course>(context)
    {
        public async Task<Course> AddAsync(AddUpdateCourseRequest addCourseRequest)
        {
            var newCourse = new Course()
            {
                Name = addCourseRequest.Name,
                Description = addCourseRequest.Description,
                AuthorId = addCourseRequest.AuthorId,
                Price = addCourseRequest.Price ?? 0,
                ModulesHaveOrder = addCourseRequest.ModulesHaveOrder,
                MinimalCompletionPercentage = addCourseRequest.MinimalCompletionPercentage
            };
            foreach (var themeId in addCourseRequest.ThemesIds)
            {
                var theme = await context.Themes.FindAsync(themeId);
                newCourse.Themes.Add(theme);
            }
            await context.Courses.AddAsync(newCourse);
            await context.SaveChangesAsync();
            return newCourse;
        }

        public async Task<Course> UpdateAsync(AddUpdateCourseRequest updateCourseRequest)
        {
            var oldDbCourse = await context.Courses.Include(x => x.Themes).FirstOrDefaultAsync(x => x.Id == updateCourseRequest.Id);
            if (oldDbCourse == null) throw new NotFoundException("Не найден курс");
            oldDbCourse.Themes.Clear();
            oldDbCourse.Name = updateCourseRequest.Name;
            oldDbCourse.Description = updateCourseRequest.Description;
            oldDbCourse.ModulesHaveOrder = updateCourseRequest.ModulesHaveOrder;
            oldDbCourse.MinimalCompletionPercentage = updateCourseRequest.MinimalCompletionPercentage;
            oldDbCourse.Price = updateCourseRequest.Price ?? 0;
            foreach (var themeId in updateCourseRequest.ThemesIds)
            {
                var theme = await context.Themes.FindAsync(themeId);
                oldDbCourse.Themes.Add(theme);
            }
            await context.SaveChangesAsync();
            return oldDbCourse;
        }
    }
}