using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using API.Service;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;

namespace API.Controllers
{
    [ApiController]
    public class UserController(UserRepository userRepository, AuthService authService) : ControllerBase
    {
        [HttpGet("users")]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await userRepository.FindAllAsync());
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            return Ok(await authService.SignInAsync(request));
        }
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] AddUserRequest request)
        {
            return Ok(await authService.SignUpAsync(request));
        }
    }
}