using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace API.Features.Groups
{
    [ApiController]
    [Route("groups")]
    [Authorize("OnlyDev")]
    public class GroupController(IGroupRepository groupRepository, CoursesDbContext context) : Controller
    {
        [HttpGet("{groupId:int}/users")]
        public async Task<IActionResult> GroupUsers(int groupId)
        {
            var data = await context.Groups.AsNoTracking().Where(x=> x.Id == groupId)
            .Select(x => new
            {
                UsersInfo = x.UserInGroups.Where(u => u.JoinStatusId == 1).Select(u => new
                {
                    u.UserId,
                    u.User.UserInformation.FirstName,
                    u.User.UserInformation.LastName,
                    u.User.UserInformation.MiddleName,
                })
            }).FirstOrDefaultAsync();
            return Ok(data.UsersInfo);
        }

        [HttpPost("/users/{userId:long}/add-course-group/{groupId:int}")]
        [Authorize]
        public async Task<IActionResult> AddUserToCourseGroup(long userId, int courseId, int groupId)
        {
            var group = await context.Groups.FindAsync(groupId);
            var nowCount = await context.UserInGroups.AsNoTracking().CountAsync(x => x.GroupId == groupId
                && x.JoinStatusId == 1);
            if (nowCount < group.MaxMembersCount)
            {
                await context.UserInGroups.AddAsync(new UserInGroup()
                {
                    UserId = userId,
                    GroupId = group.Id,
                    JoinStatusId = 1
                });
                await context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("К сожалению, набор в текущую группу по этому курсу уже закрыт");
            }
        }
        
        [HttpGet("by-user/{userId:long}")]
        public async Task<IActionResult> UserCreatedGroups(long userId)
        {
            var groupsData = await context.Groups.AsNoTracking()
                .Where(x => x.CuratorId == userId)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.CuratorId,
                    x.DateStart,
                    x.DateEnd,
                    x.MaxMembersCount,
                    x.CuratorFeedback,
                    CoursesList = x.Courses.Select(c => new { c.Id, c.Name }),
                    CoursesNavList = x.CoursesNavigation.Select(c => new { c.Id, c.Name }),
                    UsersInfo = x.UserInGroups
                        .Where(u => u.JoinStatusId == 1)
                        .Select(u => new
                        {
                            u.UserId,
                            u.User.UserInformation.FirstName,
                            u.User.UserInformation.LastName,
                            u.User.UserInformation.MiddleName,
                        }).ToList()
                })
                .OrderByDescending(x => x.DateStart)
                .ToListAsync();
            var groups = groupsData.Select(x => new
            {
                x.Id,
                x.Name,
                x.CuratorId,
                x.DateStart,
                x.DateEnd,
                x.MaxMembersCount,
                x.CuratorFeedback,
                CoursesInfo = x.CoursesList.Union(x.CoursesNavList).ToList(),
                x.UsersInfo
            }).ToList();
            return Ok(groups);
        }
        
        [HttpGet("by-user/{userId:long}/short")]
        public async Task<IActionResult> UserCreatedGroupsShort(long userId)
        {
            var groups = await context.Groups.AsNoTracking().Where(x => x.CuratorId == userId)
                .Select(x => new
                {
                    x.DateStart,
                    x.DateEnd,
                    x.Id,
                    x.Name,
                })
                .OrderByDescending(x => x.DateStart)
                .ToListAsync();
            return Ok(groups);
        }
        
        [HttpGet("{groupId:int}")]
        public async Task<IActionResult> GetGroupInfo(int groupId)
        {
            var x = await context.Groups.AsNoTracking()
            .Select(x=> new
            {
                x.DateStart,
                x.DateEnd,
                x.CuratorFeedback,
                x.MaxMembersCount,
                CuratorName = x.Curator.UserInformation.LastName + " " + x.Curator.UserInformation.FirstName,
                x.Id,
                x.Name,
                CoursesLinkedInfo = x.Courses.Select(c => new { c.Id, c.Name }),
                CoursesInfo = (x.CoursesNavigation.Select(c => new { c.Id, c.Name }))
            })
            .FirstOrDefaultAsync(x=> x.Id == groupId);
            
            return Ok(new
            {
                x.DateStart,
                x.DateEnd,
                x.CuratorFeedback,
                x.MaxMembersCount,
                x.CuratorName,
                x.Id,
                x.Name,
                CoursesInfo = x.CoursesInfo.Union(x.CoursesLinkedInfo).ToList(),
            });
        }

        [HttpGet("user-in/{userId:long}/past")]
        public async Task<IActionResult> PastGroupsWhereUserIn(long userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var data = context.UserInGroups.AsNoTracking().Where(x => x.UserId == userId && x.Group.DateEnd < today);
            var groups = await data.Select(x => new
            {
                Id = x.GroupId,
                x.Group.Name,
                x.Group.DateEnd,
                x.Group.DateStart
            }).ToListAsync();
            return Ok(groups);
        }

        [HttpGet("user-in/{userId:long}")]
        public async Task<IActionResult> PresentAndFutureGroupsWhereUserIn(long userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var data = context.UserInGroups.AsNoTracking().Where(x => x.UserId == userId && (x.Group.DateEnd >= today || x.Group.DateStart >= today));
            var groups = await data.Select(x => new
            {
                Id = x.GroupId,
                x.Group.Name,
                x.Group.DateEnd,
                x.Group.DateStart
            }).ToListAsync();
            return Ok(groups);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] GroupDto group)
        {
            var newGroup = new Group();
            context.Entry(newGroup).CurrentValues.SetValues(group);
            foreach (var course in group.Courses)
            {
                newGroup.CoursesNavigation.Add(await context.Courses.FindAsync(course.Id));
            }
            await context.Groups.AddAsync(newGroup);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] GroupDto group)
        {
            var old = await context.Groups
                .Include(x => x.CoursesNavigation)
                .FirstOrDefaultAsync(x => x.Id == group.Id);
            old.CoursesNavigation.Clear();
            context.Entry(old).CurrentValues.SetValues(group);
            foreach (var course in group.Courses)
            {
                old.CoursesNavigation.Add(await context.Courses.FindAsync(course.Id));
            }
            await context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("{groupId:int}/user/{userId:long}")]
        public async Task<IActionResult> AddUser(int groupId, long userId)
        {
            var u = await context.Users.FindAsync(userId);
            if (u == null)
            {
                return NotFound("Пользователь не найден");
            }
            await context.UserInGroups.AddAsync(new UserInGroup()
            {
                UserId = userId,
                GroupId = groupId,
                JoinStatusId = 1
            });
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{groupId:int}/user/{userId:long}")]
        public async Task<IActionResult> DeleteUserFromGroup(int groupId, long userId)
        {
            var uIn =  await context.UserInGroups.FindAsync(userId,groupId);
            if (uIn  == null)
            {
                return NoContent();
            }
            context.UserInGroups.Remove(uIn);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{groupId:int}")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var g =  await context.Groups.FindAsync(groupId);
            if (g  == null)
            {
                return NoContent();
            }
            context.Groups.Remove(g);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}