using FluentValidation;

namespace Atlas.Application.Features.Onboarding.Commands.CompleteOnboarding;

public class CompleteOnboardingCommandValidator : AbstractValidator<CompleteOnboardingCommand>
{
    public CompleteOnboardingCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty()
            .WithMessage("UserId cannot be empty.");
        RuleFor(v => v.JobTitle).NotEmpty().WithMessage("Job title cannot be empty.").MaximumLength(100)
            .WithMessage("JobTitle cannot be longer than 100 characters.");
        RuleFor(v => v.Profession).IsInEnum().WithMessage("Invalid profession selected.");
        RuleFor(v => v.Answers).NotEmpty().WithMessage("At least one answer must be provided.");
    }
}