using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.SetDefault;

public record SetDefaultWorkspaceCommand(Guid WorkspaceId) : IRequest;