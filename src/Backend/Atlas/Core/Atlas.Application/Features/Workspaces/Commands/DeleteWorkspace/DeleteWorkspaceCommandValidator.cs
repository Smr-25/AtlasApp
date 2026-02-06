using FluentValidation;

namespace Atlas.Application.Features.Workspaces.Commands.DeleteWorkspace;

public class DeleteWorkspaceCommandValidator : AbstractValidator<DeleteWorkspaceCommand>
{
    public DeleteWorkspaceCommandValidator()
    {
        RuleFor(x => x.WorkspaceId).NotEmpty()
            .WithMessage("WorkspaceId is required.");
    }
}

