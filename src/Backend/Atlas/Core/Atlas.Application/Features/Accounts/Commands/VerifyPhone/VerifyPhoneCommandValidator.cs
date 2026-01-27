using FluentValidation;

namespace Atlas.Application.Features.Accounts.Commands.VerifyPhone;

public class VerifyPhoneCommandValidator : AbstractValidator<VerifyPhoneCommand>
{
    public VerifyPhoneCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\+\d{1,3}\d{4,14}(?:x.+)?$")
            .WithMessage("Phone number must be in valid international format (e.g., +994501234567).");

        RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .WithMessage("Verification code is required.")
            .Length(6)
            .WithMessage("Verification code must be 6 digits.")
            .Matches(@"^\d{6}$")
            .WithMessage("Verification code must contain only digits.");
    }
}
