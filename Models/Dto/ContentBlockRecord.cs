using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public record ContentBlockRecord (long Id, string Name, int ContentBlockTypeId, long LessonId, string? TextValue,
         string? FileName, int Order);
}