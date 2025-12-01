using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Entities;

namespace API.Repositories
{
    public class TaskRepository(CoursesDbContext context) : BaseRepository<Models.Entities.Task>(context)
    {
        
    }
}