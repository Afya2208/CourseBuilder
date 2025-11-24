using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Models.Entities;

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
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] Theme theme)
        {
            return Ok(await themeRepository.AddAsync(theme));
        }
    }
}