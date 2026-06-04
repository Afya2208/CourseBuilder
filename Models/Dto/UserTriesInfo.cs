using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class UserTriesInfo
    {
        public long UserId { get; set; }
        public long LessonId { get; set; }
        public int SumScore { get; set; }
    }
}