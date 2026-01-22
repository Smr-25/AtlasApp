using Atlas.Application.Dtos.Users;
using FluentValidation;

namespace Atlas.Application.Validators.User;

public class UserReverifyPhoneDtoValidator : AbstractValidator<UserReverifyPhoneDto>
{
    public UserReverifyPhoneDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");
    }
}