using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Exceptions;

namespace API.Features.Courses
{
    [ApiController]
    [Route("courses")]
    [Authorize("OnlyDev")]
    public class CourseController(ICourseService courseService) : ControllerBase
    {
        [HttpGet("{courseId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseDto>> ReadById(int courseId)
        {
            return Ok(await courseService.ReadByIdAsync(courseId));
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<CoursesDtoList>> PagingSearch(int pageSize, string? text, int? themeId, int pageNumber)
        {
            return Ok(await courseService.PagingSearchAsync(pageSize, pageNumber, text, themeId));
        }

        [HttpPatch("{courseId:int}/switch-publicity")]
        [Authorize("AdminOrDev")]
        public async Task<ActionResult> ChangePublicity(int courseId)
        {
            await courseService.ChangePublicityAsync(courseId);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> Add([FromBody] AddUpdateCourseRequest request)
        {
            return Ok(await courseService.AddAsync(request));
        }

        [HttpPut("{courseId:int}")]
        public async Task<ActionResult<CourseDto>> Update([FromRoute] int courseId, [FromBody] AddUpdateCourseRequest request)
        {
            return Ok(await courseService.UpdateAsync(courseId, request));
        }
        
        [HttpDelete("{courseId:int}")]
        public async Task<ActionResult> Delete(int courseId)
        {
            await courseService.DeleteAsync(courseId);
            return NoContent();
        }
    }
}