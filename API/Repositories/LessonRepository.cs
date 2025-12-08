using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace API.Repositories
{
    public class LessonRepository(CoursesDbContext context) : BaseRepository<Lesson>(context)
    {
        public async Task<int?> GetLessonsCountForCourse(int courseId)
        {
            return context.Modules.Where(x=>x.CourseId == courseId).Include(x=>x.Lessons).Sum(x=>x.Lessons.Count);
        }
         public async Task<int?> GetLessonsCountForModule(long moduleId)
        {
            return context.Lessons.Count(x=>x.ModuleId == moduleId);
        }
    }
}