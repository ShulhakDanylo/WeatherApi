using MediatR;
using Weather.BLL.DTOs.Users;
using Weather.BLL.Helpers; 

namespace Weather.BLL.Commands.Users.Register;

public record RegisterUserCommand(RegisterUserRequestDto RequestDto) 
    : IRequest<Result<AuthResponseDto>>;