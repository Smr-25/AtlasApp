using FluentValidation;

namespace Atlas.Application.Features.Onboarding.Commands.AddOption;

public class AddOnboardingOptionCommandValidator : AbstractValidator<AddOnboardingOptionCommand>
{
    public AddOnboardingOptionCommandValidator()
    {
        RuleFor(v => v.QuestionId).NotEmpty()
            .WithMessage("QuestionId cannot be empty.");
        RuleFor(v => v.Text).NotEmpty()
            .WithMessage("Option text cannot be empty.")
            .MaximumLength(200).WithMessage("Option text cannot be longer than 200 characters.");
    }
}