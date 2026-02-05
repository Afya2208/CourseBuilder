using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class CorrelationDto
    {
        public long Id { get; set; }

        public string Left { get; set; } = null!;

        public string Right { get; set; } = null!;

        public long? TaskId { get; set; }

    }
}