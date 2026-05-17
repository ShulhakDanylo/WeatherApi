using AutoMapper;
using MediatR;
using Weather.BLL.DTOs.Users;
using Weather.BLL.Helpers;
using Weather.BLL.Interfaces;
using Weather.DAL.Repositories.Interfaces.Base;

namespace Weather.BLL.Commands.Users.Login;

public class LogInUserHandler : IRequestHandler<LogInUserCommand, Result<LogInUserResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public LogInUserHandler(IRepositoryWrapper repositoryWrapper, ITokenService tokenService, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<Result<LogInUserResponseDto>> Handle(LogInUserCommand request, CancellationToken cancellationToken)
    {
       
        var user = await _repositoryWrapper.Users.GetByEmailAsync(request.RequestDto.Email);
        
        if (user == null)
        {
            return "Неправильний Email або пароль";
        }
        var token = _tokenService.GenerateJwtToken(user);

        return new LogInUserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            AccessToken = token
        };
    }
}