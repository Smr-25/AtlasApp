using FluentValidation;

namespace Atlas.Application.Features.PersonaStates.Commands.UpdateFocusLevel;

public class UpdateFocusLevelCommandValidator : AbstractValidator<UpdateFocusLevelCommand>
{
    public UpdateFocusLevelCommandValidator()
    {
        RuleFor(x => x.Level)
            .InclusiveBetween(1, 10)
            .WithMessage("Focus level must be between 1 and 10.");
    }
}
