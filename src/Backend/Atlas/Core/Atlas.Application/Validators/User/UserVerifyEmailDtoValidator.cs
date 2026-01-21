using Atlas.Application.Dtos.Users;
using FluentValidation;

namespace Atlas.Application.Validators.User;

public class UserVerifyEmailDtoValidator : AbstractValidator<UserVerifyEmailDto>
{
    public UserVerifyEmailDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .Length(4, 8).WithMessage("Verification code must be between 4 and 8 characters long.");
    }
}