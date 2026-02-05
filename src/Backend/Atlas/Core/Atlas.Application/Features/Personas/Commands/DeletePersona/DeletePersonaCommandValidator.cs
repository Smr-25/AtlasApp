using FluentValidation;

namespace Atlas.Application.Features.Personas.Commands.DeletePersona;

public class DeletePersonaCommandValidator : AbstractValidator<DeletePersonaCommand>
{
    public DeletePersonaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Persona ID is required.");
    }
}
