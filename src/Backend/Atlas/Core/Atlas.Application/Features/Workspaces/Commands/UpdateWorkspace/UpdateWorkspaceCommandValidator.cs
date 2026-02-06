using FluentValidation;

namespace Atlas.Application.Features.Workspaces.Commands.UpdateWorkspace;

public class UpdateWorkspaceCommandValidator : AbstractValidator<UpdateWorkspaceCommand>
{
    public UpdateWorkspaceCommandValidator()
    {
        RuleFor(x => x.WorkspaceId).NotEmpty()
            .WithMessage("WorkspaceId is required.");
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");
    }
}