using Atlas.Domain.Enums;
using FluentValidation;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.AddIntegration;

public record AddIntegrationCommand(
    Guid PersonaId,
    string Name,
    IntegrationProvider Provider,
    string? Metadata
) : IRequest<Guid>;

public class AddIntegrationCommandValidator : AbstractValidator<AddIntegrationCommand>
{
    public AddIntegrationCommandValidator()
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