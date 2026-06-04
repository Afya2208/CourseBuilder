using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class LessonInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int MaxScore { get; set; }
        public int ModuleOrder { get; set; }

        public int LessonOrder { get; set; }

        public string ModuleName { get; set; }
    }
}