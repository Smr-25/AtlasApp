using Atlas.Application.Dtos.Users.Auth;
using FluentValidation;

namespace Atlas.Application.Validators.User.Auth;

public class UserReverifyPhoneDtoValidator : AbstractValidator<UserReverifyPhoneDto>
{
    public UserReverifyPhoneDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");
    }
}