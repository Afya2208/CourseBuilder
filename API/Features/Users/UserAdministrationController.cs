using API.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Users;

[ApiController]
[Route("users")]
[Authorize(Roles = "Администратор")]
public class UserAdministrationController(IAuthService authService, IUserService userService) : Controller
{
    [HttpPost("change-email")]
    public async Task<ActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
    {
        await authService.ChangeEmailAsync(request);
        return NoContent();
    }
    
    [HttpPost("import/csv")]
    public async Task<ActionResult> ImportUsersCsv([FromForm] CsvFile csvFile)
    {
        await userService.ImportStudentsUsersCsvAsync(csvFile);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<UsersDtoList>> PagingSearch([FromQuery] int pageSize, [FromQuery] string? text, [FromQuery] int? roleId, [FromQuery] int pageNumber)
    {
        var users = await userService.PagingSearchUsersAsync(pageSize, pageNumber, text, roleId);
        return Ok(users);
    }
}