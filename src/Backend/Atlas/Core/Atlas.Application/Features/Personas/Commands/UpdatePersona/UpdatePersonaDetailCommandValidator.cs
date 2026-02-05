using FluentValidation;

namespace Atlas.Application.Features.Personas.Commands.UpdatePersona;

public class UpdatePersonaDetailCommandValidator : AbstractValidator<UpdatePersonaDetailCommand>
{
    public UpdatePersonaDetailCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Persona ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Persona name is required.")
            .MaximumLength(100).WithMessage("Persona name cannot exceed 100 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Persona bio cannot exceed 500 characters.");
    }
}