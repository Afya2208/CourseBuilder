namespace Models.Dto;

public class UserDto
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public RoleDto Role { get; set; } = null!;

    public UserInformationDto? UserInformation { get; set; }
}