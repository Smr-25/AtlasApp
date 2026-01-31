using FluentValidation;

namespace Atlas.Application.Features.Constraints.Commands.UpdateConstraint;

public class UpdateConstraintCommandValidator : AbstractValidator<UpdateConstraintCommand>
{
    public UpdateConstraintCommandValidator()
    {
        RuleFor(x => x.ConstraintId)
            .NotEmpty().WithMessage("ConstraintId is required.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.ImpactLevel)
            .InclusiveBetween(1, 10).WithMessage("ImpactLevel must be between 1 and 10.")
            .When(x => x.ImpactLevel is not null);
    }
}