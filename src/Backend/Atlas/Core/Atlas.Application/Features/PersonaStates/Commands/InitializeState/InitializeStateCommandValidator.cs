using FluentValidation;

namespace Atlas.Application.Features.PersonaStates.Commands.InitializeState;

public class InitializeStateCommandValidator : AbstractValidator<InitializeStateCommand>
{
    public InitializeStateCommandValidator()
    {
        RuleFor(x => x.PersonaId)
            .NotEmpty()
            .WithMessage("PersonaId cannot be empty.");
        RuleFor(x => x.LifePhase)
            .IsInEnum()
            .WithMessage("Invalid LifePhase value.");
        
        RuleFor(x => x.MentalLoad)
            .IsInEnum()
            .WithMessage("Invalid MentalLoadLevel value.");
        
        RuleFor(x => x.EnergyLevel)
            .InclusiveBetween(0, 100)
            .WithMessage("EnergyLevel must be between 0 and 100.");
        
        RuleFor(x => x.FocusLevel)
            .InclusiveBetween(0, 100)
            .WithMessage("FocusLevel must be between 0 and 100.");
    }
}