using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;

public record CreateWorkspaceCommand(string Name, string? Description, string? LocalFolderPath) : IRequest<Guid>;
