using API.Dto;
using API.Features.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Domain.Exceptions;

namespace API.Features.Modules
{
    [ApiController]
    [Route("modules")]
    [Authorize("OnlyDev")]
    public class ModuleController(IModuleService moduleService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("/courses/{courseId:int}/modules")]
        public async Task<ActionResult<List<ModuleDto>>> ReadAllCourseModules([FromRoute] int courseId)
        {
            return Ok(await moduleService.ReadAllCourseModulesAsync(courseId));
        }
        
        [HttpGet("{moduleId:long}")]
        public async Task<ActionResult<ModuleDto>> ReadModuleById([FromRoute] long moduleId)
        {
            return Ok(await moduleService.ReadByIdAsync(moduleId));
        }
       
        [HttpPost]
        public async Task<ActionResult<ModuleDto>> Add([FromBody] ModuleDto moduleDto)
        {
            return Ok(await moduleService.AddAsync(moduleDto));
        }
        
        [HttpPost("/course/{courseId:int}/modules/import-xlsx")]
        public async Task<ActionResult> ImportModules([FromRoute] int courseId, [FromForm] XlsxFile xlsxFile)
        {
            await moduleService.ImportModulesFromXlsxAsync(xlsxFile, courseId);
            return NoContent();
        }
        
        [HttpPost("order")]
        public async Task<ActionResult> SaveModulesOrder([FromBody] IEnumerable<ModuleOrderDto> modulesOrder)
        {
            await moduleService.SaveModulesOrderAsync(modulesOrder);
            return NoContent();
        }
        
        [HttpPut("{moduleId:long}")]
        public async Task<ActionResult<ModuleDto>> Update(long moduleId, [FromBody] ModuleDto moduleDto)
        {
            return Ok(await moduleService.UpdateAsync(moduleId, moduleDto));
        }
        
        [HttpDelete("{moduleId:long}")]
        public async Task<ActionResult> Delete(long moduleId)
        {
            await moduleService.DeleteAsync(moduleId);
            return NoContent();
        }
    }
}