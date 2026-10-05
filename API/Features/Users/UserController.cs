using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Users
{
    [ApiController]
    [Route("users")]
    //[Authorize]
    public class UserController(IUserService userService, IAuthService authService) : ControllerBase
    {
        [HttpGet("{userId:long}")]
        public async Task<ActionResult<UserDto>> FindById(long userId)
        {
            return Ok(await userService.FindUserByIdWithData(userId));
        }
        
        [AllowAnonymous]
        [HttpPost("/sign-in")]
        public async Task<ActionResult<SignInResponse>> SignIn([FromBody] SignInRequest request)
        {
            return Ok(await authService.SignInAsync(request));
        }
        
        [AllowAnonymous]
        [HttpPost("/sign-up")]
        public async Task<ActionResult> SignUp([FromBody] SignUpRequest request)
        {
            await authService.SignUpAsync(request);
            return NoContent();
        }
        
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            await authService.ChangePasswordAsync(request);
            return NoContent();
        }
        
        [HttpDelete("{userId:long}")]
        public async Task<ActionResult> DeleteUser([FromRoute] long userId)
        {
            await userService.DeleteUserAsync(userId);
            return NoContent();
        }
        
        [HttpPut("{userId:long}")]
        public async Task<ActionResult> UpdateUser([FromRoute] long userId, [FromBody] UpdateUserRequest request)
        {
            await userService.UpdateUserInformationAsync(userId, request);
            return NoContent();
        }
    }
}