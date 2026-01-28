using FluentValidation;

namespace Atlas.Application.Features.PersonaStates.Commands.UpdatePhase;

public class UpdatePhaseCommandValidator : AbstractValidator<UpdatePhaseCommand>
{
    public UpdatePhaseCommandValidator()
    {
        RuleFor(x => x.NewPhase)
            .IsInEnum().WithMessage("Invalid life phase.");
        
        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Note cannot exceed 500 characters.");
    }
}