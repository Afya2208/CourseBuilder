using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using Models.Entities;

namespace API.Repositories
{
    public class UserRepository(CoursesDbContext context, RoleRepository roleRepository) : BaseRepository<User>(context)
    {
       public async Task<User> FindById(long userId)
        {
           var user = await base.FindByIdAsync(userId);
           if (user == null) throw new NotFoundException($"Не найден пользователь с id={userId}");
            user.Role = await roleRepository.FindByIdAsync(user.RoleId);
            return user;
        }
        
    }
}