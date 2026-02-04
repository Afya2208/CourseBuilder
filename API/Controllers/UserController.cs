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
    public class UserController(UserRepository userRepository, AuthService authService,
        RoleRepository roleRepository) : ControllerBase
    {

        [HttpGet("roles")]
        [Authorize(Roles="Администратор")]
        public async Task<IActionResult> FindAllRoles()
        {
            var roles = await roleRepository.FindAllAsync();
            var rolesDto = roles.ConvertAll(x=> x.ToDto());
            return Ok(rolesDto);
        }

        [HttpGet("users")]
        [Authorize(Roles="Администратор")]
        public async Task<IActionResult> FindAll()
        {
            var users = await userRepository.FindAllAsync([x => x.UserInformation, x=>x.Role]);
            var usersDto = users.ConvertAll(x=> x.ToDto());
            return Ok(usersDto);
        }
        [HttpGet("users/{userId:long}")]
        [Authorize]
        public async Task<IActionResult> FindById(long userId)
        {
            User? userDb = await userRepository.FindByIdAsync(userId, [x => x.Role, x => x.UserInformation]);
            if (userDb == null) throw new NotFoundException($"Не найден пользователь id={userId}"); 
            return Ok(userDb.ToDto());
        }
        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            return Ok(await authService.SignInAsync(request));
        }

        [HttpPost("sign-up")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            var user = await authService.SignUpAsync(request);
            user.Role = await roleRepository.FindByIdAsync(user.RoleId);
            return Ok(user.ToDto());
        }
        [HttpDelete("users/{userId:long}")]
        [Authorize(Roles="Администратор")]
        public async Task<IActionResult> Update(long userId)
        {
            return Ok((await userRepository.DeleteAsync(userId)).ToDto());
        }
        [HttpPut("users")]
        [Authorize(Roles="Администратор")]
        public async Task<IActionResult> Update([FromBody] UserDto userToUpdate)
        {
            return Ok((await userRepository.UpdateAsync(userToUpdate.ToEntity())).ToDto());
        }
    }
}