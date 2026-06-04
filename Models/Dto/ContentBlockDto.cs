using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class ContentBlockDto
    {
        public ContentBlockDto() {}
         public ContentBlockDto(long id, string name, int typeId, long lessonId, string? textValue,
         string? fileName, int order)
        {
            Id = id;
            Name = name;
            ContentBlockTypeId = typeId;
            LessonId = lessonId;
            FileName = fileName;
            TextValue = textValue;
            Order = order;
        }
        public string? Name { get; set; }

        public int ContentBlockTypeId { get; set; }

        public string? TextValue { get; set; }

        public string? FileName { get; set; }

        public long LessonId { get; set; }

        public int Order { get; set; }

        public long Id { get; set; }
    }
}