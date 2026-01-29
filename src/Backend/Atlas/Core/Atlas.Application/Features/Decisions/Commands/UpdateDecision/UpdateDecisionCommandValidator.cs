using FluentValidation;

namespace Atlas.Application.Features.Decisions.Commands.UpdateDecision;

public class UpdateDecisionCommandValidator : AbstractValidator<UpdateDecisionCommand>
{
    public UpdateDecisionCommandValidator()
    {
        RuleFor(x => x.DecisionId)
            .NotEmpty().WithMessage("Decision Id is required.");

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
        
        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be a valid enum value.");
    }
}