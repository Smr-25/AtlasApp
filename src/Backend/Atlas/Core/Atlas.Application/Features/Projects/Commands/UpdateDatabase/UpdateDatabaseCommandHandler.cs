using Atlas.Application.Common.Exceptions.Common; 
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Projects.Commands.UpdateDatabase;

public class UpdateDatabaseCommandHandler(
    IApplicationDbContext applicationDbContext,
    IMigrationBuilderService migrationBuilder,
    IScriptRunnerService scriptRunner
) : IRequestHandler<UpdateDatabaseCommand, string>
{
    public async Task<string> Handle(UpdateDatabaseCommand request, CancellationToken cancellationToken)
    {
        var project = await applicationDbContext.ProjectProfiles
            .FirstOrDefaultAsync(x => x.Id.Equals(request.ProjectId), cancellationToken);

        if (project == null) 
            throw new NotFoundException($"Project with ID {request.ProjectId} not found.");

        var command = migrationBuilder.BuildUpdateDatabaseCommand(project, request.TargetMigration);

        if (string.IsNullOrEmpty(project.RootPath) || !Directory.Exists(project.RootPath))
            throw new Exception("Project root path is invalid or missing.");

        var result = await scriptRunner.ExecuteAsync("dotnet", command.Replace("dotnet ", ""), project.RootPath, cancellationToken);

        if (result.Contains("Build failed") || result.Contains("Error") || result.Contains("fail"))
        {
            return $"❌ Database Update Failed.\n\n🛠️ Command: {command}\n\n📄 Log:\n{result}";
        }

        return $"✅ Database Updated Successfully.\n{(request.TargetMigration != null ? $"(Target: {request.TargetMigration})" : "(Latest)")}\n\n🛠️ Command: {command}\n\n📄 Log:\n{result}";
    }
}