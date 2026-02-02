using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.LinkIntegration;

public record LinkIntegrationCommand(
    Guid WorkspaceId,
    Guid IntegrationId,
    string? Config
) : IRequest<bool>;
