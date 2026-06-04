using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class TaskAnswerDto
    {
        public long Id { get; set; }

        public string? TextValue { get; set; }

        public long TaskId { get; set; }

        public long? UserId { get; set; }

        public bool? IsRight { get; set; }

    }
}