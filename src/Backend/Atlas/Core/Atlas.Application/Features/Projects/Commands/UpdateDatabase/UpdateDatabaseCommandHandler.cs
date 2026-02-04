using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Projects.Commands.UpdateDatabase;

public class UpdateDatabaseCommandHandler(
    IApplicationDbContext applicationDbContext,
    IMigrationBuilderService migrationBuilder,
    IScriptRunnerService scriptRunner)
    : IRequestHandler<UpdateDatabaseCommand, string>
{
    public async Task<string> Handle(UpdateDatabaseCommand request, CancellationToken cancellationToken)
    {
        var project = await applicationDbContext.ProjectProfiles.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (project == null) throw new NotFoundException("Project not found.");

        var command = migrationBuilder.BuildUpdateDatabaseCommand(project, request.TargetMigration);
        var result = await scriptRunner.ExecuteAsync("dotnet", command.Replace("dotnet ", ""), project.RootPath, cancellationToken);

        return $"️Command: {command}\n  Result:\n{result}";
    }
}