namespace Models.Dto;

public class ChangeEmailRequest
{
    public long UserId { get; set; }
    public string Email { get; set; }
}