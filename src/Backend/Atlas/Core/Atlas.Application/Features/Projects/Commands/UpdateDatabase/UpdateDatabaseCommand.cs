using MediatR;

namespace Atlas.Application.Features.Projects.Commands.UpdateDatabase;

public record UpdateDatabaseCommand(Guid ProjectId, string? TargetMigration) : IRequest<string>;