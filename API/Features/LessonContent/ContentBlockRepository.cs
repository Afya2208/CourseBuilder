using Domain.Entities;

namespace API.Features.LessonContent
{
    public interface IContentBlockRepository
    {
        
    }
    public class ContentBlockRepository(CoursesDbContext context) : IContentBlockRepository
    {
        
    }
}