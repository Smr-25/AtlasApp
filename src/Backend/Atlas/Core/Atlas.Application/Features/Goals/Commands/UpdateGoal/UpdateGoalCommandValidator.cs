using FluentValidation;

namespace Atlas.Application.Features.Goals.Commands.UpdateGoal;

public class UpdateGoalCommandValidator : AbstractValidator<UpdateGoalCommand>
{
    public UpdateGoalCommandValidator()
    {
        RuleFor(x => x.GoalId)
            .NotEmpty().WithMessage("GoalId is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.")
            .When(x => x.Title is not null);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 10).WithMessage("Priority must be between 1 and 10.")
            .When(x => x.Priority is not null);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("DueDate must be in the future.")
            .When(x => x.DueDate is not null);
    }
}