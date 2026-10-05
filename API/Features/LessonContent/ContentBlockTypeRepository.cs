using Domain.Entities;

namespace API.Features.LessonContent
{
    public interface IContentBlockTypeRepository
    {
        
    }
    public class ContentBlockTypeRepository(CoursesDbContext context) : IContentBlockTypeRepository
    {
        
    }
}