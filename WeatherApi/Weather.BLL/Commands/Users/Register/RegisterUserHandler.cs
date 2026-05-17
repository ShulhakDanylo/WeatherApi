using AutoMapper;
using MediatR;
using FluentValidation;
using Weather.BLL.DTOs.Users;
using Weather.BLL.Interfaces;
using Weather.DAL.Entities;
using Weather.BLL.Helpers;
using Weather.DAL.Repositories.Interfaces.Base;


namespace Weather.BLL.Commands.Users.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IValidator<RegisterUserCommand> _validator;

    public RegisterUserHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        ITokenService tokenService,
        IValidator<RegisterUserCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.First().ErrorMessage;
        }

        
        var newUser = _mapper.Map<User>(request.RequestDto);
        

        await _repositoryWrapper.Users.CreateAsync(newUser);
        await _repositoryWrapper.SaveChangesAsync();

        
        var token = _tokenService.GenerateJwtToken(newUser);
        
        return new AuthResponseDto
        {
            AccessToken = token,
            UserResponseDto = _mapper.Map<UserResponseDto>(newUser)
        };
    }
}