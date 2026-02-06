using FluentValidation;

namespace Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;

public class CreateWorkspaceCommandValidator : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");
    }
}