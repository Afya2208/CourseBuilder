using System.Security.Authentication;
using API.Util;
using Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Users
{
    public interface IAuthService
    {
        Task<SignInResponse> SignInAsync(SignInRequest request);
        Task SignUpAsync(SignUpRequest request);
        Task ChangePasswordAsync(ChangePasswordRequest request);
        Task ChangeEmailAsync(ChangeEmailRequest request);
    }
    
    public class AuthService(IUserRepository userRepository, IConfiguration configuration) : IAuthService
    {
        public async Task<SignInResponse> SignInAsync(SignInRequest request)
        {
            User? user = await userRepository.FindUserByEmailWithData(request.Email);
            if (user == null) 
                throw new AuthenticationException("Неправильный пароль или логин");

            var passwordHashFromRequest = Hashes.GetPbkdf2Hash(request.Password, user.Salt);
            if (!passwordHashFromRequest.SequenceEqual(user.Password)) 
                throw new AuthenticationException("Неправильный пароль или логин");

            return new SignInResponse(JwtTokens.GenerateToken(configuration, user), new UserDto(user));
        }
        
        public async System.Threading.Tasks.Task ChangePasswordAsync(ChangePasswordRequest request)
        {
            var newSalt = Hashes.GetNewSalt();
            var newPasswordHash = Hashes.GetPbkdf2Hash(request.Password, newSalt);
            await userRepository.ChangePassword(request.UserId, newPasswordHash, newSalt);
        }

        public async Task ChangeEmailAsync(ChangeEmailRequest request)
        {
            await userRepository.ChangeEmail(request.UserId, request.Email);
        }

        public async Task SignUpAsync(SignUpRequest request)
        { 
            if (await userRepository.DoesExistUserWithEmail(request.Email)) 
                throw new AuthenticationException("Данная почта уже используется, укажите другую");
            
            var salt = Hashes.GetNewSalt();
            var passwordHash = Hashes.GetPbkdf2Hash(request.Password, salt);
            var newUser = new User()
            {
                Salt = salt,
                Email = request.Email,
                RoleId = request.RoleId,
                Password = passwordHash,
            };
            newUser.UserInformation = request.UserInformation.MapToEntity();
            await userRepository.AddAsync(newUser);
        }
    }
}