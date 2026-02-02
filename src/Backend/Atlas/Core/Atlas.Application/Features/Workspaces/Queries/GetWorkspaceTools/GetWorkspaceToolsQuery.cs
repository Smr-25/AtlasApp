using Atlas.Application.Features.Workspaces.Dtos;
using MediatR;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaceTools;

public record GetWorkspaceToolsQuery(Guid WorkspaceId) : IRequest<List<WorkspaceToolDto>>;