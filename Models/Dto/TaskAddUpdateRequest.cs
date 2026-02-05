using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Entities;

namespace Models.Dto
{
    public class TaskAddUpdateRequest
    {
    public long Id { get; set; }

    public string? Question { get; set; }
    public string? TextAnswer { get; set; }
    public List<CorrelationDto>? Correlations { get; set; } = new();
    public List<TaskAnswerDto>? AllAnswerOptions { get; set; } = new();

    public int TaskTypeId { get; set; }

    public long LessonId { get; set; }

    public int Order { get; set; }

    }
}