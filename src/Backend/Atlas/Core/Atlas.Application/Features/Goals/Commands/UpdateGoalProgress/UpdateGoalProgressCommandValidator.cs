using FluentValidation;

namespace Atlas.Application.Features.Goals.Commands.UpdateGoalProgress;

public class UpdateGoalProgressCommandValidator : AbstractValidator<UpdateGoalProgressCommand>
{
    public UpdateGoalProgressCommandValidator()
    {
        RuleFor(x => x.GoalId)
            .NotEmpty().WithMessage("GoalId is required.");

        RuleFor(x => x.ProgressPercentage)
            .InclusiveBetween(0, 100).WithMessage("ProgressPercentage must be between 0 and 100.");
    }
}