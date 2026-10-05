using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace API.Features.Users
{
    public interface IRoleRepository
    {
        public Task<List<RoleDto>> ReadAllAsync();
    }
    
    public class RoleRepository(CoursesDbContext context) : IRoleRepository
    {
        public async Task<List<RoleDto>> ReadAllAsync()
        {
            return await context.Roles.Select(r => new RoleDto(r.Id, r.Name)).ToListAsync();
        }
    }
}