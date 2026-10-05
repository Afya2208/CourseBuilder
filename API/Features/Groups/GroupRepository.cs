using API.Features.Learning;
using API.Util;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Groups
{
    public interface IGroupRepository
    {
        Task<List<UserInGroupDto>> ReadUsersInGroupAsync(int groupId);
        Task<string?> ReadGroupNameByIdAsync(int groupId);
    }
    public class GroupRepository(CoursesDbContext context) : IGroupRepository
    {
        public async Task<List<UserInGroupDto>> ReadUsersInGroupAsync(int groupId)
        {
            return await context.UserInGroups
                .AsNoTracking()
                .Where(g => g.GroupId == groupId && g.JoinStatusId == 1)
                .OrderBy(g => g.User.UserInformation.LastName)
                .Select(g => new UserInGroupDto(g.UserId, g.User.UserInformation.LastName, g.User.UserInformation.FirstName))
                .ToListAsync();
        }

        public async Task<string?> ReadGroupNameByIdAsync(int groupId)
        {
            return await context.Groups.Where(x=>x.Id == groupId).Select(x=>x.Name).FirstOrDefaultAsync();
        }
    }
}