using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class KitDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public long AuthorId { get; set; }

        public IEnumerable<CourseInfo>? CoursesInfo { get; set; }

    }

    public class CourseInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}