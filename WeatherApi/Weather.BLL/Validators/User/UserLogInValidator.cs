using FluentValidation;
using Weather.BLL.Commands.Users.Login;
using Weather.BLL.Constants;

namespace Weather.BLL.Validators.User;

public class LoginUserValidator : AbstractValidator<LogInUserCommand>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.RequestDto.Email)
            .NotEmpty().WithMessage(UserConstants.EmailRequiredErrorMessage)
            .EmailAddress().WithMessage(UserConstants.EmailIncorrectErrorMessage);
    }
}