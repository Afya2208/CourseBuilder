using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Kits
{
    [ApiController]
    [Route("kits")]
    [Authorize(Roles = "Администратор")]
    public class KitController(IKitRepository kitRepository) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<KitDto>> AddKit([FromBody] KitDto kitDto)
        {
            var kitSaved = await kitRepository.AddAsync(kitDto);
            return Created(string.Empty, new KitDto(kitSaved));
        }

        [HttpPut("{kitId:int}")]
        public async Task<ActionResult<KitDto>> UpdateKit([FromRoute] int kitId, [FromBody] KitDto kitDto)
        {
            var updatedKit = await kitRepository.UpdateAsync(kitId, kitDto);
            return Ok(new KitDto(updatedKit));
        }
        
        [HttpDelete("{kitId:int}")]
        public async Task<ActionResult> DeleteKit(int kitId)
        {
            await kitRepository.DeleteAsync(kitId);
            return NoContent();
        }
        
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<ActionResult<KitsDtoList>> PagingSearch(int pageSize, string? text, int? themeId, int pageNumber)
        {
            return Ok(await kitRepository.PagingSearchAsync(pageSize, pageNumber, text, themeId));
        }
    }
}