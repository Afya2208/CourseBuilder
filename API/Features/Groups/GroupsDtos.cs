using Domain.Entities;

namespace API.Features.Groups;

public record CourseInfoForGroupDto(int Id, string Name);

public record GroupDto(int Id, string Name, long CuratorId, DateOnly DateStart, 
    DateOnly DateEnd, int MaxMembersCount, string? CuratorFeedback, ICollection<CourseInfoForGroupDto> Courses);


public static class GroupsDtos
{
    
}