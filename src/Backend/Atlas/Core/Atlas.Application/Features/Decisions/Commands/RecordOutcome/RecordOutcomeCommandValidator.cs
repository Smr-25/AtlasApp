using FluentValidation;

namespace Atlas.Application.Features.Decisions.Commands.RecordOutcome;

public class RecordOutcomeCommandValidator : AbstractValidator<RecordOutcomeCommand>
{
    public RecordOutcomeCommandValidator()
    {
        RuleFor(x => x.DecisionId)
            .NotEmpty().WithMessage("DecisionId is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid DecisionStatus enum value.");
        
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
        
        RuleFor(x => x.WasExpected)
            .NotNull().WithMessage("WasExpected must be provided.");
        
        RuleFor(x => x.LessonLearned)
            .MaximumLength(2000).WithMessage("LessonLearned cannot exceed 2000 characters.");
        
    }
}