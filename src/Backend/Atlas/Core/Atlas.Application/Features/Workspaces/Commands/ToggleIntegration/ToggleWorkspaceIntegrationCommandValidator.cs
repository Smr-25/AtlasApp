using FluentValidation;

namespace Atlas.Application.Features.Workspaces.Commands.ToggleIntegration;

public class ToggleWorkspaceIntegrationCommandValidator : AbstractValidator<ToggleWorkspaceIntegrationCommand>
{
    public ToggleWorkspaceIntegrationCommandValidator()
    {
        RuleFor(x => x.WorkspaceId).NotEmpty()
            .WithMessage("WorkspaceId is required.");
        RuleFor(x => x.IntegrationId).NotEmpty()
            .WithMessage("IntegrationId is required.");
    }
}