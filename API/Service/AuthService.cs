using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using API.Interface;
using API.Repositories;
using API.Util;
using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Models.Entities;

namespace API.Service
{
    public class AuthService(UserRepository userRepository, IConfiguration configuration) : IAuthService
    {
        public async Task<SignInResponse> SignInAsync(SignInRequest request)
        {
            User? user = await userRepository.FindByCondition(x=>x.Email == request.Email);
            if (user == null) throw new AuthenticationException("Неправильный пароль или логин");

            var passwordHashFromRequest = Hashes.GetPbkdf2Hash(request.Password, user.Salt);
            if (passwordHashFromRequest != user.Password) throw new AuthenticationException("Неправильный пароль или логин");

            var token =  JwtTokens.GenerateToken(configuration, user);
            return new SignInResponse()
            {
                Token = token,
                LastName = user.UserInformation.LastName,
                
            };
        }
        public async Task<bool> SignUpAsync(AddUserRequest request)
        {
            // проверка на почту, если уже есть пользователь, то отклоняем запрос
            User? user = await userRepository.FindByCondition(x=>x.Email == request.Email);
            if (user != null) throw new ArgumentException("Данная почта уже используется, укажите другую");
            // теперь хешируем пароль и сохраняем нового пользователя
            var salt = Hashes.GetNewSalt();
            var passwordHash = Hashes.GetPbkdf2Hash(request.Password, salt);
            var newUser = new User()
            {
                Salt = salt,
                Email = request.Email,
                Password = passwordHash,

            };
            var userInfo = new UserInformation()
            {
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                FirstName = request.FirstName,
                Phone = request.Phone,
                Position = request.Position,
                User = newUser,
            };
            newUser.UserInformation = userInfo;
            await userRepository.AddAsync(newUser);
            return true;
        }
    }
}