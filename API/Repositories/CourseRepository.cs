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
        public async System.Threading.Tasks.Task ChangePublicityAsync(int courseId)
        {
            var course = await context.Courses.FindAsync(courseId);
            course.IsPublic = !course.IsPublic;
            await context.SaveChangesAsync();
        }
        public async System.Threading.Tasks.Task AddToUserAsync(long userId, int courseId)
        {
            await context.UserHasCourses.AddAsync(new UserHasCourse()
            {
                UserId = userId, CourseId = courseId
            });
            await context.SaveChangesAsync();
        }
        
    }
}