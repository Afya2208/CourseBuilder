using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Models.Entities;

namespace Models.Dto
{
    public class TaskDto
    {
        public long Id { get; set; }

        public string Question { get; set; } = null!;

        public int TaskTypeId { get; set; }

        public long LessonId { get; set; }

        public int Order { get; set; }

    }
}