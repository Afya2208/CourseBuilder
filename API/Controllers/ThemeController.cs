using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("themes")]
    public class ThemeController(ThemeRepository themeRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await themeRepository.FindAllAsync());
        }
    }
}