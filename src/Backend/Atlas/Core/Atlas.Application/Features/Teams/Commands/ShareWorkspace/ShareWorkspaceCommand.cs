using MediatR;

namespace Atlas.Application.Features.Teams.Commands.ShareWorkspace;

public record ShareWorkspaceCommand(Guid TeamId, Guid WorkspaceId) : IRequest<bool>;

