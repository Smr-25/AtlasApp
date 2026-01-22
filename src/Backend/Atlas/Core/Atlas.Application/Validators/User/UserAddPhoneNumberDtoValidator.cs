using Atlas.Application.Dtos.Users;
using FluentValidation;

namespace Atlas.Application.Validators.User;

public class UserAddPhoneNumberDtoValidator : AbstractValidator<UserAddPhoneNumberDto>
{
    public UserAddPhoneNumberDtoValidator()
    {
        RuleFor(x=>x.Email).NotEmpty()
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Invalid phone number format.");
    }
}
