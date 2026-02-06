using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.ToggleIntegration;

public record ToggleWorkspaceIntegrationCommand(
    Guid WorkspaceId, 
    Guid IntegrationId, 
    bool Enable
) : IRequest;