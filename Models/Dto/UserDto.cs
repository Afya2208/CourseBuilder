using System.Text.Json.Serialization;

namespace Models.Dto;

public class UserDto
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Password { get; set; }
    public RoleDto? Role { get; set; } 
    public int RoleId { get; set; }

    public UserInformationDto UserInformation { get; set; }
}