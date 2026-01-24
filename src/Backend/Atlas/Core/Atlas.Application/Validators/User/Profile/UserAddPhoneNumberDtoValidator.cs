using Atlas.Application.Dtos.Users.Profile;
using FluentValidation;

namespace Atlas.Application.Validators.User.Profile;

public class UserAddPhoneNumberDtoValidator : AbstractValidator<UserAddPhoneNumberDto>
{
    public UserAddPhoneNumberDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Invalid phone number format. Use international format (e.g., +994501234567).");
        
        RuleFor(x => x.UserVerificationChannel)
            .IsInEnum().WithMessage("Invalid verification channel.");
    }
}