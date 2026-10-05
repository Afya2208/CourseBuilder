using API.Features.Courses;
using API.Features.Kits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;

namespace API.Features.Learning;

[ApiController]
[Route("learn")]
[Authorize("AllUsers")]
public class UserLearningController(IKitService kitService, ICourseService courseService) : Controller
{
    [HttpPost("user/{userId:long}/kits/{kitId:int}")]
    public async Task<ActionResult> AddKitToUser(long userId, int kitId)
    {
        await kitService.AddKitToUserAsync(userId, kitId);
        return NoContent();
    }

    [HttpGet("user/{userId:long}/kits")]
    public async Task<ActionResult<List<KitShortDto>>> GetUserKits(long userId)
    {
        return Ok(await kitService.ReadAllKitsUserHasAsync(userId));
    }
    
    [HttpGet("user/{userId:long}/kits-ids")]
    public async Task<ActionResult<List<int>>> GetUserKitsId(long userId)
    {
        return Ok(await kitService.ReadAllKitsIdUserHasAsync(userId));
    }
    
    [HttpGet("user/{userId:long}/courses")]
    public async Task<ActionResult<List<CourseDto>>> FindAllUserAvailableCourses(long userId)
    {
        return Ok(await courseService.ReadAllCoursesUserHasAsync(userId));
    }
    
    [HttpPost("user/{userId:long}/courses/{courseId:int}")]
    public async Task<ActionResult> AddToUserAsync(long userId, int courseId)
    {
        await courseService.AddCourseToUserAsync(userId, courseId);
        return NoContent();
    }
    
    [HttpGet("user/{userId:long}/courses-ids")]
    public async Task<ActionResult<List<int>>> GetCourseIdsAvailableForUser(long userId)
    {
        return Ok(await courseService.ReadAllCoursesIdUserHasAsync(userId));
    }
    
    [HttpGet("user/{userId:long}/courses-short")]
    public async Task<ActionResult<List<CourseShortDto>>> AvailableCoursesNames(long userId)
    {
        return Ok(await courseService.ReadAllCoursesShortUserHasAsync(userId));
    }
}