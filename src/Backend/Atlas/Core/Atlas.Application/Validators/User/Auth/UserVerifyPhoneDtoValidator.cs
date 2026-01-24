using Atlas.Application.Dtos.Users.Auth;
using FluentValidation;

namespace Atlas.Application.Validators.User.Auth;

public class UserVerifyPhoneDtoValidator : AbstractValidator<UserVerifyPhoneDto>
{
    public UserVerifyPhoneDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Phone number format is invalid.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Verification code is required.")
            .Length(4, 8)
            .WithMessage("Verification code must be between 4 and 8 characters long.");
    }
}