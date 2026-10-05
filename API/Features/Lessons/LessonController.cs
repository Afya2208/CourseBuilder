using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Exceptions;

namespace API.Features.Lessons;

[ApiController]
[Route("lessons")]
//[Authorize("OnlyDev")]
public class LessonController(ILessonService lessonService, CoursesDbContext context) : ControllerBase
{
    [HttpGet("{lessonId:long}")]
    [Authorize("AllUsers")]
    public async Task<ActionResult<LessonDto>> ReadById(long lessonId)
    {
        return Ok(await lessonService.ReadByIdAsync(lessonId));
    }

    [HttpGet("{lessonId:long}/check-for/{userId:long}")]
    [Authorize("AllUsers")]
    public async Task<ActionResult<bool>> CheckIsLessonDoneByUser(long lessonId, long userId)
    {
        return Ok(await lessonService.IsLessonDoneByUserAsync(lessonId, userId));
    }

    [HttpGet("{lessonId:long}/prev")]
    //[Authorize("AllUsers")]
    public async Task<ActionResult<long?>> GetPreviousLessonId(long lessonId)
    {
        var lessonLink = await lessonService.GetPreviousLessonIdAsync(lessonId);
        if (lessonLink.LessonId == null) return NoContent();
        return Ok($"/modules/{lessonLink.ModuleId}/lessons/{lessonLink.LessonId}");
    }
    [HttpGet("{lessonId:long}/next")]
    //[Authorize("AllUsers")]
    public async Task<ActionResult<long?>> GetNextLessonId(long lessonId)
    {
        var lessonLink = await lessonService.GetNextLessonIdAsync(lessonId);
        if (lessonLink.LessonId == null) return NoContent();
        return Ok($"/modules/{lessonLink.ModuleId}/lessons/{lessonLink.LessonId}");
    }

    [HttpGet("/modules/{moduleId:long}/lessons")]
    [Authorize("AllUsers")]
    public async Task<ActionResult<List<LessonDto>>> ReadAllByModuleId(long moduleId)
    {
        return Ok(await lessonService.ReadAllByModuleIdAsync(moduleId));
    }
    
    [HttpPost("order")]
    public async Task<ActionResult> SaveLessonsOrder([FromBody] List<LessonOrderDto> lessonOrders)
    {
        await lessonService.SaveLessonsOrderAsync(lessonOrders);
        return NoContent();
    }
    
    [HttpPost]
    public async Task<ActionResult<LessonDto>> AddLesson([FromBody] LessonDto lessonToAdd)
    {
        return Ok(await lessonService.AddAsync(lessonToAdd));
    }
    
    [HttpPut("{lessonId:long}")]
    public async Task<ActionResult<LessonDto>> Update([FromRoute] long lessonId, [FromBody] LessonDto lessonToUpdate)
    {
         return Ok(await lessonService.UpdateAsync(lessonId, lessonToUpdate));
    }
    
    [HttpDelete("{lessonId:long}")]
    public async Task<ActionResult> Delete(long lessonId)
    {
        await lessonService.DeleteAsync(lessonId);
        return NoContent();
    }
}
