using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Users;

[ApiController]
[Route("roles")]
[Authorize("OnlyAdmin")]
public class RoleController(IRoleService roleService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<List<RoleDto>>> ReadAllRoles()
    {
        return Ok(await roleService.ReadAllAsync());
    }
}