using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    string Name,
    ProjectType Type,
    string RootPath,
    string? StartupPath,  
    string? MigrationPath   
) : IRequest<Guid>;