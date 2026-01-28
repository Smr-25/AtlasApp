using FluentValidation;

namespace Atlas.Application.Features.PersonaStates.Commands.UpdateEnergyLevel;

public class UpdateEnergyLevelCommandValidator : AbstractValidator<UpdateEnergyLevelCommand>
{
    public UpdateEnergyLevelCommandValidator()
    {
        RuleFor(x => x.Level)
            .InclusiveBetween(1, 10)
            .WithMessage("Energy level must be between 1 and 10.");
    }
}
