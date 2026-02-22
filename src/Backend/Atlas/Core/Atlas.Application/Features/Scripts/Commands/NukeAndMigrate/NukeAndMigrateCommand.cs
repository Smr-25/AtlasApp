using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.NukeAndMigrate;

public record NukeAndMigrateCommand(string ConnectionString, string MigrationsProjectPath) : IRequest<string>;

