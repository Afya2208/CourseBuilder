using API.Features.Courses;
using API.Features.Kits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Developing;

[ApiController]
[Route("dev")]
[Authorize("AdminOrDev")]
public class DeveloperContentController(IKitService kitService, ICourseService courseService) : Controller
{
    [HttpGet("{userId:long}/kits")]
    public async Task<ActionResult<List<KitDto>>> FindAllKitsByUser(long userId)
    {
        return Ok(await kitService.ReadAllKitsByUserIdAsync(userId));
    }
    
    [HttpGet("{userId:long}/courses/{courseId:int}/check")]
    public async Task<ActionResult<bool>> CheckIsUserCourseAuthor(long userId, int courseId)
    {
        return Ok(await courseService.CheckIsUserCourseAuthorAsync(userId, courseId));
    }
    
    [HttpGet("{userId:long}/courses")]
    public async Task<ActionResult<List<CourseDto>>> FindAllCoursesByUser(long userId)
    {
        return Ok(await courseService.ReadAllCoursesByUserIdAsync(userId));
    }
    
    [HttpGet("{userId:long}/courses-short")]
    public async Task<IActionResult> FindAllCoursesByUserShort(long userId)
    {
        return Ok(await courseService.ReadAllCoursesShortByUserIdAsync(userId));
    }
}