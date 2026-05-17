using FluentValidation;
using Weather.BLL.Commands.Users.Register;

namespace Weather.BLL.Validators.User;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator(BaseUserValidator userValidator)
    {
        RuleFor(x => x.RequestDto).SetValidator(userValidator);
    }
}