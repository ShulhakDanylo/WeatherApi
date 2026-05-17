namespace Weather.BLL.DTOs.Users;

public class AuthResponseDto
{
    public string AccessToken { get; set; }
    public UserResponseDto UserResponseDto { get; set; } = new UserResponseDto();
}