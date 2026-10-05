using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Themes
{
    [ApiController]
    [Route("themes")]
    [Authorize(Roles="Разработчик")]
    public class ThemeController(IThemeService themeService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<ThemeDto>>> ReadAllThemes()
        {
            return Ok(await themeService.ReadAllAsync());
        }
        
        [HttpPost]
        public async Task<ActionResult<ThemeDto>> AddTheme([FromBody] ThemeDto themeDto)
        {
            return Created(string.Empty, await themeService.AddAsync(themeDto));
        }
        
        [HttpPut("{themeId:int}")]
        public async Task<ActionResult<ThemeDto>> Update([FromRoute] int themeId, [FromBody] ThemeDto themeToUpdate)
        {
             return Ok(await themeService.UpdateAsync(themeId, themeToUpdate));
        }
        
        [HttpDelete("{themeId:int}")]
        public async Task<ActionResult> Delete([FromRoute] int themeId)
        {
            await themeService.DeleteAsync(themeId);
            return NoContent();
        }
    }
}