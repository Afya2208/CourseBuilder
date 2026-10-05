namespace API.Features.Learning;

public record UserTryInfoDto(long UserId, long LessonId, int SumScore);

public record GroupProgressDto(Stream FileStream, string GroupName, string CourseName);
public record UserInGroupDto(long UserId, string LastName, string FirstName);

public record IsCourseDoneByUser(int CourseId, bool IsNeededToStateCompleted);

public record CourseProgressDto(double Percent,
    int SolvedTasksCount, int TotalTasksCount, bool IsCourseCompleted, string Status);

public static class ProgressesDtos
{
    
}