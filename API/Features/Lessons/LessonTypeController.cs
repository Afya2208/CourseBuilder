using Microsoft.AspNetCore.Mvc;

namespace API.Features.Lessons;

[ApiController]
[Route("lesson-types")]
public class LessonTypeController(ILessonTypeRepository lessonTypeRepository) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonTypeDto>>> ReadAllTypes()
    {
        return Ok(await lessonTypeRepository.ReadAllAsync());
    }
}