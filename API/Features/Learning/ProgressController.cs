using API.Features.Lessons;
using API.Features.Users;
using API.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.StaticFiles;

namespace API.Features.Learning
{
    [ApiController]
    [Authorize("AllUsers")]
    [Route("progress")]
    public class ProgressController(IProgressService progressService) : Controller
    {
        [HttpPost("user/{userId:long}/try/{lessonId:long}")]
        public async Task<ActionResult> UserTriedLessonTasks(long userId, long lessonId, [FromBody] List<UserTrySolveDetail> details)
        {
            await progressService.SaveUserTryLessonAsync(userId, lessonId, details);
            return NoContent();
        }

        [HttpGet("group/{groupId:int}/course/{courseId:int}/xlsx")]
        public async Task<ActionResult> GroupProgress(int groupId, int courseId)
        {
            var groupProgressDto = await progressService.GetGroupProgressInCourseXlsxAsync(groupId, courseId);
            new FileExtensionContentTypeProvider().TryGetContentType("xlsx", out var type);
            return File(groupProgressDto.FileStream, type, $"Успеваемость {groupProgressDto.GroupName} {groupProgressDto.CourseName}.xlsx");
        }
        
        [HttpGet("user/{userId:long}/lesson/{lessonId:long}/unused-tries")]
        public async Task<ActionResult<int?>> LeftTries(long userId, long lessonId)
        {
            int? triesCount = await progressService.GetUnusedTriesAsync(userId, lessonId);
            if (triesCount == null)
                return NoContent();
            return Ok(triesCount);
        }
        [HttpGet("user/{userId:long}/lesson/{lessonId:long}/check")]
        public async Task<ActionResult<bool>> Check(long userId, long lessonId)
        {
            return Ok(await progressService.CheckIsLessonDoneByUserAsync(userId, lessonId));
        }
        
        [HttpGet("{userId:long}/course/{courseId:int}")]
        public async Task<ActionResult<CourseProgressDto>> GetCourseProgress(long userId, int courseId)
        {
            return Ok(await progressService.GetCourseProgressForUserAsync(userId, courseId));
        }
        
        [HttpPost("user/{userId:long}/lesson/{lessonId:long}/try")]
        public async Task<ActionResult> AddTryForUser(long userId, long lessonId)
        {
            await progressService.AddAdditionalTryToUserAsync(userId, lessonId);
            return NoContent();
        }
    }
}