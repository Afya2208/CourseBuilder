namespace API.Features.LessonContent;


public record ContentBlockDto(long Id, string? Name, int ContentBlockTypeId, string? TextValue,
    string? FileName, long LessonId, int Order);

public record ContentBlockFormData(long Id, string? Name, int ContentBlockTypeId, 
    string? TextValue, IFormFile? FormFile, long LessonId, int Order);


public static class ContentBlocksDtos
{
    
}