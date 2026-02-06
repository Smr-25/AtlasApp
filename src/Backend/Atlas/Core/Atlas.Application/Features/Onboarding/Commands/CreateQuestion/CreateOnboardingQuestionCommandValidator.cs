using FluentValidation;

namespace Atlas.Application.Features.Onboarding.Commands.CreateQuestion;

public class CreateOnboardingQuestionCommandValidator : AbstractValidator<CreateOnboardingQuestionCommand>
{
    public CreateOnboardingQuestionCommandValidator()
    {
        RuleFor(v => v.Text)
            .NotEmpty().WithMessage("Question text cannot be empty.")
            .MaximumLength(200).WithMessage("Question text cannot exceed 200 characters.");

        RuleFor(v => v.Order)
            .GreaterThan(0).WithMessage("Order must be a positive integer.");
    }
}