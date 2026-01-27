using FluentValidation;

namespace Atlas.Application.Features.Accounts.Commands.AddPhoneNumber;

public class AddPhoneNumberCommandValidator : AbstractValidator<AddPhoneNumberCommand>
{
    public AddPhoneNumberCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required.")
            .Matches(@"^\+\d{1,3}\d{4,14}(?:x.+)?$").WithMessage("PhoneNumber must be in valid international format.");

        RuleFor(x => x.VerificationChannel)
            .IsInEnum().WithMessage("VerificationChannel must be a valid enum value.");
    }
}