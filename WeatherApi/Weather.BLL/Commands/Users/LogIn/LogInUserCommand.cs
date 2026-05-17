using MediatR;
using Weather.BLL.DTOs.Users; 
using Weather.BLL.Helpers;

namespace Weather.BLL.Commands.Users.Login;

public record LogInUserCommand(LogInUserRequestDto RequestDto) 
    : IRequest<Result<LogInUserResponseDto>>;