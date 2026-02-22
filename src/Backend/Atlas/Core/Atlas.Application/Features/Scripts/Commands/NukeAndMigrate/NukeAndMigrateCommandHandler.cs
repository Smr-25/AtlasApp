using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.NukeAndMigrate;

public class NukeAndMigrateCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<NukeAndMigrateCommand, string>
{
    public async Task<string> Handle(NukeAndMigrateCommand request, CancellationToken cancellationToken)
    {
        var dropResult = await scriptRunner.ExecuteAsync(
            "dotnet", "ef database drop --force", request.MigrationsProjectPath, cancellationToken);

        var migrateResult = await scriptRunner.ExecuteAsync(
            "dotnet", "ef database update", request.MigrationsProjectPath, cancellationToken);

        return $"DROP: {dropResult}\nMIGRATE: {migrateResult}";
    }
}

