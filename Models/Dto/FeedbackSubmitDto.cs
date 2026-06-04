using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class FeedbackSubmitDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? UserId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Email { get; set; }

        public string Title { get; set; } = null!;
       
        public DateTime DateTimeSent { get; set; }

        public DateTime? DateTimeSolved { get; set; }
        public string Text { get; set; } = null!;

        public int FeedbackCategoryId { get; set; }
        
    }
}