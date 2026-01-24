using Atlas.Application.Dtos.Users.Auth;
using FluentValidation;

namespace Atlas.Application.Validators.User.Auth;

public class UserReverifyEmailDtoValidator : AbstractValidator<UserReverifyEmailDto>
{
    public UserReverifyEmailDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid email address.");
    }
}