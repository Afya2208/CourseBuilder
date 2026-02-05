using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Models.Dto
{
    public class ContentBlockFormData
    {
        public string? Name { get; set; }

        public int ContentBlockTypeId { get; set; }

        public string? TextValue { get; set; }

        public string? FileName { get; set; }
        public IFormFile? File { get; set; }

        public long LessonId { get; set; }

        public int Order { get; set; }

        public long Id { get; set; }

    }
}