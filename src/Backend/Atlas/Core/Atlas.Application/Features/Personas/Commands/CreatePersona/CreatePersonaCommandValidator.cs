using FluentValidation;

namespace Atlas.Application.Features.Personas.Commands.CreatePersona;

public class CreatePersonaCommandValidator : AbstractValidator<CreatePersonaCommand>
{
    public CreatePersonaCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Persona name is required.")
            .MaximumLength(100).WithMessage("Persona name must not exceed 100 characters.");

        RuleFor(x => x.Alias)
            .MaximumLength(50).WithMessage("Persona alias must not exceed 50 characters.");
    }
}