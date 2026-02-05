using FluentValidation;

namespace Atlas.Application.Features.Personas.Commands.SetPrimaryPersona;

public class SetPrimaryPersonaCommandValidator : AbstractValidator<SetPrimaryPersonaCommand>
{
    public SetPrimaryPersonaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Persona ID is required.");
    }
}
