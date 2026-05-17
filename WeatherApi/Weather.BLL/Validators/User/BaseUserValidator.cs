using FluentValidation;
using Weather.BLL.Constants;
using Weather.BLL.DTOs.Users;


namespace Weather.BLL.Validators.User;

public class BaseUserValidator : AbstractValidator<RegisterUserRequestDto>
{
    public BaseUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(UserConstants.EmailRequiredErrorMessage)
            .EmailAddress().WithMessage(UserConstants.EmailIncorrectErrorMessage);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(UserConstants.UserNameRequiredErrorMessage);
    }
}