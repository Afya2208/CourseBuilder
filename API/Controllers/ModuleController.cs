using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class ModuleController(ModuleRepository moduleRepository) : ControllerBase
    {
        [HttpGet("courses/{courseId:int}/modules")]
        public async Task<IActionResult> FindAllByCourseId(int courseId)
        {
            List<Module> foundModules = await moduleRepository.FindAllByConditionAsync(x => x.CourseId == courseId);
            return Ok(foundModules);
        }
    }
}