using System.Text.RegularExpressions;
using FluentValidation;

namespace Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;

public class CreateWorkspaceCommandValidator : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator()
    {
        RuleFor(x => x.PersonaId)
            .NotEmpty().WithMessage("Persona ID cannot be empty.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Workspace name cannot be empty.")
            .MaximumLength(100).WithMessage("Workspace name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Color)
            .Must(color => color == null || Regex.IsMatch(color, "^#([A-Fa-f0-9]{6})$"))
            .WithMessage("Color must be a valid hex color code (e.g., #FF5733).");
    }
}