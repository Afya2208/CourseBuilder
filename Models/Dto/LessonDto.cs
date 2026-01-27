
using Models.Entities;

namespace Models.Dto;
public partial class LessonDto
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int LessonTypeId { get; set; }

    public long ModuleId { get; set; }

    public int Order { get; set; }

}