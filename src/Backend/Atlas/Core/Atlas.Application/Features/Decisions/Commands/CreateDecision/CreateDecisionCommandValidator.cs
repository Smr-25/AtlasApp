using FluentValidation;

namespace Atlas.Application.Features.Decisions.Commands.CreateDecision;

public class CreateDecisionCommandValidator : AbstractValidator<CreateDecisionCommand>
{
    public CreateDecisionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
        
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be a valid enum value.")
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.GoalId)
            .NotEmpty().WithMessage("GoalId is required.");
    }
}