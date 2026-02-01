using Atlas.Domain.Enums;
using FluentValidation;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.CreatePersona;

public record CreatePersonaCommand(
    string Name,
    PersonaType PersonaType,
    string? Bio,
    bool IsPrimary
) : IRequest<Guid>;


public class CreatePersonaCommandValidator : AbstractValidator<CreatePersonaCommand>
{
    public CreatePersonaCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Persona name cannot be empty.")
            .MaximumLength(100).WithMessage("Persona name cannot exceed 100 characters.");

        RuleFor(x => x.PersonaType)
            .IsInEnum().WithMessage("Invalid persona type.");
        
        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.");
    }
}