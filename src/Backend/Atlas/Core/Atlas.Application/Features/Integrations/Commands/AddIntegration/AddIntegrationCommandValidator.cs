using FluentValidation;

namespace Atlas.Application.Features.Integrations.Commands.AddIntegration;

public class AddIntegrationCommandValidator : AbstractValidator<AddIntegrationCommand>
{
    public AddIntegrationCommandValidator()
    {
        RuleFor(x => x.PersonaId)
            .NotEmpty().WithMessage("Persona ID cannot be empty.");

        RuleFor(x => x.Provider)
            .IsInEnum().WithMessage("Invalid integration provider.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Integration name cannot be empty.")
            .MaximumLength(100).WithMessage("Integration name cannot exceed 100 characters.");

        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token cannot be empty.");
    }
}