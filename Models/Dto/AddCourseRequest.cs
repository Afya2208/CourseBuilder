using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class AddUpdateCourseRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Price { get; set; }
        public bool ModulesHaveOrder { get; set; }
        public long AuthorId { get; set; }
        public List<int> ThemesIds {get;set;} 
        public int MinimalCompletionPercentage { get; set; }
    }
}