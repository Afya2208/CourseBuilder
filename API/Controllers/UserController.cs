using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Exceptions;
using API.Repositories;
using API.Service;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    public class UserController(UserRepository userRepository, AuthService authService,
        RoleRepository roleRepository, CoursesDbContext context) : ControllerBase
    {

        [HttpGet("roles")]
        [Authorize(Roles="Администратор")]
        public async Task<IActionResult> FindAllRoles()
        {
            var roles = await roleRepository.FindAllAsync();
            var rolesDto = roles.ConvertAll(x=> x.ToDto());
            return Ok(rolesDto);
        }
        
        [HttpGet("users/{userId:long}")]
        [Authorize]
        public async Task<IActionResult> FindById(long userId)
        {
            User? userDb = await userRepository.FindByIdAsync(userId, [x => x.Role, x => x.UserInformation]);
            if (userDb == null) throw new NotFoundException($"Не найден пользователь id={userId}"); 
            return Ok(userDb.ToDto());
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            return Ok(await authService.SignInAsync(request));
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            var user = await authService.SignUpAsync(request);
            user.Role = await roleRepository.FindByIdAsync(user.RoleId);
            return Ok(user.ToDto());
        }
        [HttpPost("users/change-email")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
        {
            var user = await context.Users.FirstOrDefaultAsync(x=>x.Email == request.Email);
            if (user != null)
            {
                return BadRequest("Почта уже занята, выберите другую почту");
            }
            var userToChange = await context.Users.FindAsync(request.UserId);
            userToChange.Email = request.Email;
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("users/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            await authService.ChangePassword(request);
            return Ok();
        }
        [HttpDelete("users/{userId:long}")]
        [Authorize(Roles="Администратор")]
        public async Task<IActionResult> Update(long userId)
        {
            return Ok((await userRepository.DeleteAsync(userId)).ToDto());
        }
        [HttpPut("users")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UserDto userToUpdate)
        {
            var userDbType = userToUpdate.ToEntity();
            return Ok((await userRepository.UpdateAsync(userDbType)).ToDto());
        }

        [HttpPost("users/import/csv")]
       
        public async Task<IActionResult> ImportUsersCsv([FromForm] CsvFile csvFile)
        {
            var file = csvFile.FormFile;
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var text = Encoding.UTF8.GetString(stream.ToArray()).Trim();
            var lines = text.Split("\n");
            
            var titlesLine = lines[0];
            var titles = titlesLine.Split(",", StringSplitOptions.TrimEntries);
            if (titles[0].Equals("email", StringComparison.CurrentCultureIgnoreCase)
            && titles[1].Equals("пароль", StringComparison.CurrentCultureIgnoreCase) 
            && titles[2].Equals("фамилия", StringComparison.CurrentCultureIgnoreCase)
            && titles[3].Equals("имя", StringComparison.CurrentCultureIgnoreCase)
            && titles[4].Equals("отчество", StringComparison.CurrentCultureIgnoreCase))
            {
                for (int i = 1; i < lines.Length; i++)
                {
                    var elements = lines[i].Split(',', StringSplitOptions.TrimEntries);
                    if (elements.Length > 3 && elements.Length < 6)
                    {
                        var email = elements[0];
                        var password = elements[1];
                        var lastName = elements[2];
                        var firstName = elements[3];
                        string? middleName = elements.Length == 5 ? elements[4] : null;

                        var emailIsUnused = (await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email)) == null;
                        if (emailIsUnused)
                        {
                            var salt = Hashes.GetNewSalt();
                            var newUser = new User()
                            {
                                RoleId = 3,
                                Email = email,
                                Salt = salt,
                                Password = Hashes.GetPbkdf2Hash(password, salt),
                                UserInformation = new UserInformation()
                                {
                                    LastName = lastName,
                                    FirstName = firstName,
                                    MiddleName = middleName
                                }
                            };
                            await context.Users.AddAsync(newUser);
                        }
                        else
                        {
                            return BadRequest($"Почта {email} занята, выберите другую почту");
                        }
                    }
                    else
                    {
                        return BadRequest("Неправильный формат содержания файла. На каждой строчке через запятую должны быть указаны корректные данные, не менее 4 значений (отчество необязательно)");
                    }
                }
                await context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("Неправильный формат заголовков файла. Они должны быть следующими (через запятую): " 
                + "email, пароль, фамилия, имя, отчество");
            }
        }
        
        [HttpGet("/users/search")]
        public async Task<IActionResult> PagingSearch(int pageSize, string? text, int? roleId, int pageNumber)
        {
            var query = context.Users.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(x => EF.Functions.ILike(x.Email + x.UserInformation.LastName + " " + x.UserInformation.FirstName + " " + x.UserInformation.MiddleName, $"%{text}%"));
            }
            if (roleId != null)
            {
                query = query.Where(x => x.RoleId == roleId);
            }
            var totalCount = await query.CountAsync();
            var users = await query.Include(x=>x.Role)
                .Include(x=>x.UserInformation)
                .OrderBy(x=>x.Id)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Email,
                    x.Role,
                    x.UserInformation
                })
                .ToListAsync();
            return Ok(new
            {
                totalCount,
                users
            });
        }
    }
}