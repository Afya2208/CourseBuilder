using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Entities;

namespace API.Repositories
{
    public class ModuleRepository(CoursesDbContext context) : BaseRepository<Module>(context)
    {
         public async Task<int?> GetModulesCountForCourse(int courseId)
        {
            return context.Modules.Count(x=>x.CourseId == courseId);
        }
    }
}