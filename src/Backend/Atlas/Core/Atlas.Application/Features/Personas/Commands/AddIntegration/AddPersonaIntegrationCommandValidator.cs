using FluentValidation;

namespace Atlas.Application.Features.Personas.Commands.AddIntegration;

public class AddPersonaIntegrationCommandValidator : AbstractValidator<AddPersonaIntegrationCommand>
{
    public AddPersonaIntegrationCommandValidator()
    {
        RuleFor(x => x.PersonaId)
            .NotEmpty().WithMessage("Persona ID cannot be empty.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Integration name cannot be empty.")
            .MaximumLength(100).WithMessage("Integration name cannot exceed 100 characters.");

        RuleFor(x => x.Provider)
            .IsInEnum().WithMessage("Invalid integration provider.");
    }
}