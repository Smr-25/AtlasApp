using FluentValidation;

namespace Atlas.Application.Features.Constraints.Commands.CreateConstraint;

public class CreateConstraintCommandValidator : AbstractValidator<CreateConstraintCommand>
{
    public CreateConstraintCommandValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid constraint type.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.ImpactLevel)
            .InclusiveBetween(1, 10).WithMessage("Impact level must be between 1 and 10.");
    }
}