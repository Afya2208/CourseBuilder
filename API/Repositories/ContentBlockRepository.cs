using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Models.Entities;

namespace API.Repositories
{
    public class ContentBlockRepository(CoursesDbContext context) : BaseRepository<ContentBlock>(context)
    {
        
    }
}