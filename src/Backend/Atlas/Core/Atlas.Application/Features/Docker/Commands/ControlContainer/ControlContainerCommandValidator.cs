using FluentValidation;

namespace Atlas.Application.Features.Docker.Commands.ControlContainer;

public class ControlContainerCommandValidator : AbstractValidator<ControlContainerCommand>
{
    public ControlContainerCommandValidator()
    {
        RuleFor(x => x.ContainerId)
            .NotEmpty().WithMessage("Container ID is required.")
            .MaximumLength(64).WithMessage("Container ID cannot exceed 64 characters.");

        RuleFor(x => x.Action)
            .IsInEnum().WithMessage("Invalid Docker action.");
    }
}
