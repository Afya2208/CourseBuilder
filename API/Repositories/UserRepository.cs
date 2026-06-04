using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace API.Repositories
{
    public class UserRepository(CoursesDbContext context) : BaseRepository<User>(context)
    {
        public async Task<User> UpdateAsync(User user)
        {
            var oldUser = await context.Users
            .Include(x=>x.UserInformation)
            .FirstOrDefaultAsync(x => x.Id == user.Id);
            oldUser.UserInformation = user.UserInformation;
            oldUser.RoleId = user.RoleId;
            await context.SaveChangesAsync();
            oldUser.Role = await context.Roles.FindAsync(oldUser.RoleId);
            return oldUser;
        }
    }
}