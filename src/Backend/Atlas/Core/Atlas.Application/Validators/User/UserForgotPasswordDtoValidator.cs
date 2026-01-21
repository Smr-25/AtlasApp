using Atlas.Application.Dtos.Users;
using FluentValidation;

namespace Atlas.Application.Validators.User;

public class UserForgotPasswordDtoValidator : AbstractValidator<UserForgotPasswordDto>
{
    public UserForgotPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format.");

        RuleFor(x => x.UserName)
            .MinimumLength(3)
            .When(x => !string.IsNullOrEmpty(x.UserName))
            .WithMessage("Username must be at least 3 characters long.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.UserName))
            .WithMessage("Either Email or UserName must be provided.");
    }
}