using Atlas.Application.Features.Workspaces.Dtos;
using MediatR;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaceById;

public record GetWorkspaceByIdQuery(Guid WorkspaceId) : IRequest<WorkspaceDto>;

