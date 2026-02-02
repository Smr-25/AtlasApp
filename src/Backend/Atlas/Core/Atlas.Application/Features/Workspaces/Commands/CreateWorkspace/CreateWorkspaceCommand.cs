using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;

public record CreateWorkspaceCommand(
    Guid PersonaId,
    string Name,
    string? Description,
    string? Icon,
    string? Color,
    bool IsDefault
) : IRequest<Guid>;