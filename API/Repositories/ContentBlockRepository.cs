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
        public async Task<List<ContentBlockDto>> FindAllByLessonId(long lessonId)
        {
            var contents = await context.ContentBlocks
            .Where(x=>x.LessonId == lessonId)
            .OrderBy(x=>x.Order)
            .Select(x=> new ContentBlockDto(x.Id, x.Name, x.ContentBlockTypeId,
            x.LessonId, x.TextValue, x.FileName, x.Order))
            .ToListAsync();
            return contents;
        }
    }
}