using Atlas.Application.Features.Workspaces.Dtos;
using MediatR;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaces;

public record GetWorkspacesQuery : IRequest<List<WorkspaceDto>>;
