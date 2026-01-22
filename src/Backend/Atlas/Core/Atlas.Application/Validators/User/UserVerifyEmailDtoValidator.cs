using Atlas.Application.Dtos.Users;
using FluentValidation;

namespace Atlas.Application.Validators.User;

public class UserVerifyEmailDtoValidator : AbstractValidator<UserVerifyEmailDto>
{
    public UserVerifyEmailDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .Length(6).WithMessage("Verification code must be 6 characters long.");
    }
}