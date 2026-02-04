using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Projects.Commands.AddMigration;

public class AddMigrationCommandHandler(
    IApplicationDbContext applicationDbContext,
    IMigrationBuilderService migrationBuilder,
    IScriptRunnerService scriptRunner)
    : IRequestHandler<AddMigrationCommand, string>
{
    public async Task<string> Handle(AddMigrationCommand request, CancellationToken cancellationToken)
    {
        var project = await applicationDbContext.ProjectProfiles.FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);
        if (project == null) throw new NotFoundException("Project not found.");

        var migrationName = request.CustomMigrationName;

        if (string.IsNullOrEmpty(migrationName))
        {
            var migFolder = project.MigrationProjectPath ?? project.RootPath; 
            migrationName = migrationBuilder.GenerateNextMigrationName(migFolder);
        }

        var command = migrationBuilder.BuildAddMigrationCommand(project, migrationName);
        var result = await scriptRunner.ExecuteAsync("dotnet", command.Replace("dotnet ", ""), project.RootPath, cancellationToken);

        if (result.Contains("Build failed") || result.Contains("Error"))
            return $"🛠️ Command: {command}\n  Result:\n{result}";
        project.LastMigrationName = migrationName;
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return $"🛠️ Command: {command}\n  Result:\n{result}";
    }
}