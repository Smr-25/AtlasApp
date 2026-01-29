using FluentValidation;

namespace Atlas.Application.Features.Personas.Commands.UpdatePersona;

public class UpdatePersonaCommandValidator : AbstractValidator<UpdatePersonaCommand>
{
    public UpdatePersonaCommandValidator()
    {
        RuleFor(x => x.PersonaId)
            .NotEmpty().WithMessage("Persona ID is required.");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Persona name is required.")
            .MaximumLength(100).WithMessage("Persona name must not exceed 100 characters.");

        RuleFor(x => x.Alias)
            .MaximumLength(50).WithMessage("Persona alias must not exceed 50 characters.");
    }
}