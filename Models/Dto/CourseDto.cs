using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class CourseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        public decimal? Price { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? AuthorId { get; set; }
        
        public int? LinkedGroupId { get; set; }
        public bool IsPublic { get; set; }
        public bool ModulesHaveOrder { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ThemeDto>? Themes {get;set;} 

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ModulesCount { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? LessonsCount { get; set; }
        public List<int>? ThemesIds {get;set;} 

    }
}