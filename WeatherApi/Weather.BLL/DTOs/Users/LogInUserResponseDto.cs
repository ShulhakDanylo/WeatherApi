namespace Weather.BLL.DTOs.Users;

public class LogInUserResponseDto
{
    public long Id { get; set; }
    public required string AccessToken { get; set; }
    public required string Email { get; set; }
    
}