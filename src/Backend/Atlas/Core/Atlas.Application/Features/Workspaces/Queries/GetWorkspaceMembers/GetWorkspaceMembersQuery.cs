using Atlas.Application.Features.Workspaces.Dtos;
using MediatR;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaceMembers;

public record GetWorkspaceMembersQuery(Guid WorkspaceId) : IRequest<List<WorkspaceMemberDto>>;

