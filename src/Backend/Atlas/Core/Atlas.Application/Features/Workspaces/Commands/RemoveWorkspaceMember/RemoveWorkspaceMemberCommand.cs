using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.RemoveWorkspaceMember;

public record RemoveWorkspaceMemberCommand(Guid WorkspaceId, Guid UserId) : IRequest;

