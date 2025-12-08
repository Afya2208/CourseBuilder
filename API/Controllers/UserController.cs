using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Repositories;
using API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    public class UserController(UserRepository userRepository, AuthService authService) : ControllerBase
    {
        [HttpGet("users")]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await userRepository.FindAllAsync());
        }
        [HttpGet("users/{userId:long}")]
        public async Task<IActionResult> FindById(long userId)
        {
            User? userDb = await userRepository.FindByIdAsync(userId, [x => x.Role, x => x.UserInformation]);
            if (userDb == null) throw new NotFoundException($"Не найден пользователь id={userId}"); 
            return Ok(userDb.ToFullDto());
        }
        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            return Ok(await authService.SignInAsync(request));
        }
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            return Ok(await authService.SignUpAsync(request));
        }
    }
}