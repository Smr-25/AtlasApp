using FluentValidation;

namespace Atlas.Application.Features.Accounts.Commands.VerifyEmail;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.");

        RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .WithMessage("Verification code is required.")
            .Length(6)
            .WithMessage("Verification code must be 6 digits.")
            .Matches(@"^\d{6}$")
            .WithMessage("Verification code must contain only digits.");
    }
}
