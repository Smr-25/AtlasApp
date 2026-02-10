using FluentValidation;

namespace Atlas.Application.Features.Workspaces.Commands.SetDefault;

public class SetDefaultWorkspaceCommandValidator : AbstractValidator<SetDefaultWorkspaceCommand>
{
    public SetDefaultWorkspaceCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("WorkspaceId is required.");
    }
}

