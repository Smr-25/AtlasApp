using Atlas.Domain.Enums;
using FluentValidation;

namespace Atlas.Application.Features.Accounts.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(20)
            .WithMessage("Username must not exceed 20 characters.");
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MinimumLength(3)
            .WithMessage("Full name must be at least 3 characters long.")
            .MaximumLength(20)
            .WithMessage("Full name must not exceed 20 characters.");
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email is required.");
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("A valid phone number is required.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid role. Must be one of: Developer, Designer, SecOps, Marketer, TeamLeader.");
    }
}