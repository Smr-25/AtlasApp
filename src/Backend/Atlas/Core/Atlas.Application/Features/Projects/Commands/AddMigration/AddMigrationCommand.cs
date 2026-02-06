using Atlas.Domain.Entities; 
using MediatR;

namespace Atlas.Application.Features.Projects.Commands.AddMigration;

public record AddMigrationCommand(Guid ProjectId, string? CustomMigrationName) : IRequest<string>;