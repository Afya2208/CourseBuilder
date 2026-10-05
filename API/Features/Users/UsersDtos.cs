using System.Text.Json.Serialization;
using Domain.Entities;

namespace API.Features.Users;

public record RoleDto(int Id, string Name);

[method: JsonConstructor]
public record UserInformationDto (long UserId, string LastName, string FirstName,
    string? MiddleName, string? Phone, string? Position)
{
    
    public UserInformationDto(UserInformation info) : this(info.UserId, info.LastName,
        info.FirstName, info.MiddleName, info.Phone, info.Position)
    {
        
    }
    public UserInformation MapToEntity()
    {
        return new UserInformation()
        {
            UserId = UserId,
            LastName = LastName,
            FirstName = FirstName,
            MiddleName = MiddleName,
            Phone = Phone,
            Position = Position
        };
    }
}

public record UserDto(long Id, string Email, RoleDto Role, UserInformationDto UserInformation)
{
    public UserDto(User user) : this(user.Id, user.Email, new RoleDto(user.RoleId, user.Role.Name), new UserInformationDto(user.UserInformation))
    {
        
    }
}

public record UpdateUserRequest(long UserId, UserInformationDto UserInformation)
{
    
}

public record UsersDtoList(List<UserDto> Users, int TotalCount);

public record SignInRequest(string Email, string Password);

public record SignInResponse(string Token, UserDto User);

public record SignUpRequest(string Email, string Password, int RoleId,
    UserInformationDto UserInformation);

public record ChangePasswordRequest(long UserId, string Password);

public record ChangeEmailRequest(long UserId, string Email);


public static class UsersDtos
{
    public static IQueryable<UserDto> SelectDto(this IQueryable<User> query)
    {
        return query.Select(u=> new UserDto(
            u.Id, u.Email, new RoleDto(u.RoleId, u.Role.Name),
            new UserInformationDto(u.Id, u.UserInformation.LastName,
                u.UserInformation.FirstName, u.UserInformation.MiddleName, 
                u.UserInformation.Phone, u.UserInformation.Position)
            ));
    }
    
}