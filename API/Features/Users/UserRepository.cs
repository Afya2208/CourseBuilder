using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Users
{
    public interface IUserRepository
    {
        Task<User?> FindUserByEmailWithData(string email);
        Task<bool> DoesExistUserWithEmail(string email);
        Task<User?> FindUserById(long userId);
        Task<UserDto?> FindUserByIdWithData(long userId);
        Task ChangePassword(long userId, byte[] passwordHash, byte[] salt);
        Task ChangeEmail(long userId, string email);
        Task UpdateUserInformationAsync(long userId, UserInformation userData);
        Task<User> AddUser(User userToAdd);
        Task AddUsers(List<User> usersToAdd);
        Task DeleteUser(long userId);
        Task<UsersDtoList> PagingSearchUsers(int pageSize, int pageNumber, string? text, int? roleId);
        Task AddAsync(User newUser);
    }
    
    public class UserRepository(CoursesDbContext context) : IUserRepository
    {
        public async Task UpdateUserInformationAsync(long userId, UserInformation userData)
        {
            var userInformation = await context.UserInformations.FindAsync(userId);
            if (userInformation == null) 
                throw new NotFoundException("Не найден пользователь", userId);
            context.Entry(userInformation).CurrentValues.SetValues(userData);
            await context.SaveChangesAsync();
        }

        public Task<User> AddUser(User userToAdd)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DoesExistUserWithEmail(string email)
        {
            return await context.Users.AnyAsync(x=>x.Email == email);
        }
        
        public Task<User?> FindUserByEmailWithData(string email)
        {
            return context.Users.Include(x=>x.UserInformation)
                .Include(x=>x.Role)
                .FirstOrDefaultAsync(x=>x.Email == email);
        }

        public Task<User?> FindUserById(long userId)
        {
            return context.Users.FirstOrDefaultAsync(x=>x.Id == userId);
        }

        public async Task<UserDto?> FindUserByIdWithData(long userId)
        {
            return await context.Users.Where(u => u.Id == userId).SelectDto().FirstOrDefaultAsync();
        }

        public async Task ChangePassword(long userId, byte[] passwordHash, byte[] salt)
        {
            var rows = await context.Users.Where(x=> x.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u=>u.Password, passwordHash)
                    .SetProperty(u=>u.Salt, salt));
            if (rows == 0) 
                throw new NotFoundException("Не найден пользователь", userId);
        }

        public async Task ChangeEmail(long userId, string email)
        {
            var rows = await context.Users.Where(x=> x.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u=>u.Email, email));
            if (rows == 0) 
                throw new NotFoundException("Не найден пользователь", userId);
        }
        
        public async Task AddAsync(User userToAdd)
        {
            var added = await context.Users.AddAsync(userToAdd);
            await context.SaveChangesAsync();
        }

        public async Task AddUsers(List<User> usersToAdd)
        {
            await context.Users.AddRangeAsync(usersToAdd);
            await context.SaveChangesAsync();
        }

        public async Task DeleteUser(long userId)
        {
            var userToDelete = await context.Users.FindAsync(userId);
            if (userToDelete == null) 
                throw new NotFoundException("Не найден пользователь", userId);
            context.Users.Remove(userToDelete);
            await context.SaveChangesAsync();
        }

        public async Task<UsersDtoList> PagingSearchUsers(int pageSize, int pageNumber, string? text, int? roleId)
        {
            var query = context.Users.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(x =>
                    EF.Functions.ILike(
                        x.Email + x.UserInformation.LastName + " " + x.UserInformation.FirstName + " " +
                        x.UserInformation.MiddleName, $"%{text}%"));
            }
            if (roleId != null)
            {
                query = query.Where(x => x.RoleId == roleId);
            }
            var totalCount = await query.CountAsync();
            var users = await query
                .OrderBy(x => x.Id)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .SelectDto()
                .ToListAsync();
            return new UsersDtoList(users, totalCount);
        }
    }
}