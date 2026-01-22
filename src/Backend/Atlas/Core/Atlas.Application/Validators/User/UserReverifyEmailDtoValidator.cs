using Atlas.Application.Dtos.Users;
using FluentValidation;

namespace Atlas.Application.Validators.User;

public class UserReverifyEmailDtoValidator : AbstractValidator<UserReverifyEmailDto>
{
    public UserReverifyEmailDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid email address.");
    }
}