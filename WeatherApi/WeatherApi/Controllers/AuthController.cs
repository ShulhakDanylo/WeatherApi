using Microsoft.AspNetCore.Mvc;
using Weather.BLL.Commands.Users.Register;
using Weather.BLL.Commands.Users.Login;
using Weather.BLL.DTOs.Users;
using Microsoft.AspNetCore.Authorization;

namespace WeatherApi.Controllers;

public class AuthController : BaseApiController
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    public async Task<IActionResult> RegisterAsync(RegisterUserRequestDto requestDto)
    {
        return HandleResult(await Mediator.Send(new RegisterUserCommand(requestDto)));
    }
    
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] LogInUserRequestDto requestDto)
    {
        return HandleResult(await Mediator.Send(new LogInUserCommand(requestDto)));
    }
   
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout() => Ok();
}