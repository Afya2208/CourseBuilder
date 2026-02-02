namespace Models.Dto;

public class UserDto
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public RoleDto Role { get; set; } = null!;
    public int RoleId { get; set; }

    public UserInformationDto UserInformation { get; set; }
}