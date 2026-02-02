using Atlas.Application.Features.Workspaces.Dtos;
using MediatR;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspacesByPersona;

public record GetWorkspacesByPersonaQuery(Guid PersonaId) : IRequest<List<WorkspaceDto>>;