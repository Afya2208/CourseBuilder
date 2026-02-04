using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class ModuleDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
        public int CourseId { get; set; }
        public bool LessonsHaveOrder { get; set; }
        public int Order { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? LessonsCount { get; set; }
    }
}