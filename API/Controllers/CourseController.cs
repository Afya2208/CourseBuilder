using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    [Route("courses")]
    public class CourseController(CourseRepository courseRepository,
        ModuleRepository moduleRepository, LessonRepository lessonRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            List<Course> courses = await courseRepository.FindAllAsync([x=>x.Themes]);
            List<CourseDto> courseDtos = courses.ConvertAll(x=>x.ToDto());
            courseDtos.ForEach(async x=>
            {
                x.LessonsCount = await lessonRepository.GetLessonsCountForCourse(x.Id);
                x.ModulesCount = await moduleRepository.GetModulesCountForCourse(x.Id);
            });
            return Ok(courseDtos);
        }
        [HttpGet("{courseId:int}")]
        public async Task<IActionResult> FindById(int courseId)
        {
            Course? course = await courseRepository.FindByIdAsync(courseId, [x => x.Themes]);
            if (course == null) throw new NotFoundException($"Не найден курс id={courseId}");
            return Ok(course.ToDto());
        }
        [HttpGet("search")]
        public async Task<IActionResult> FindAllByTextAndThemeId(string? text, int? themeId)
        {
            var textIsNull = string.IsNullOrWhiteSpace(text);
            var themeIdIsNull = !themeId.HasValue;
            if (!textIsNull)
            {
                text = text.ToLower();
            }
            List<Course> courses = await courseRepository.FindAllByConditionAsync(x => (themeIdIsNull || x.Themes.Any(t => t.Id == themeId)) 
            && (textIsNull || x.Name.ToLower().Contains(text) || x.Description == null || x.Description.ToLower().Contains(text)) , [x=>x.Themes]);
            List<CourseDto> courseDtos = courses.ConvertAll(x=>x.ToDto());
            courseDtos.ForEach(async x=>
            {
                x.LessonsCount = await lessonRepository.GetLessonsCountForCourse(x.Id);
                x.ModulesCount = await moduleRepository.GetModulesCountForCourse(x.Id);
            });
            return Ok(courseDtos);
        }
        [HttpPost]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Add([FromBody] AddUpdateCourseRequest addCourseRequest)
        {
            return Ok(await courseRepository.AddAsync(addCourseRequest));
        }
        [HttpPut]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Update([FromBody] AddUpdateCourseRequest updateCourseRequest)
        {
             return Ok(await courseRepository.UpdateAsync(updateCourseRequest));
        }
        [HttpDelete("{courseId:int}")]
        [Authorize(Roles="Разработчик")]
        public async Task<IActionResult> Delete(int courseId)
        {
            return Ok(await courseRepository.DeleteAsync(courseId));
        }
    }
}