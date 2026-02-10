using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Services;

public class MigrationBuilderService(ILogger<MigrationBuilderService> logger) : IMigrationBuilderService
{
    public string BuildAddMigrationCommand(ProjectProfile project, string migrationName)
    {
        var command = $"dotnet ef migrations add {migrationName}";

        switch (project.Type)
        {
            case ProjectType.OnionArchitecture:
            case ProjectType.NTier:
                if (!string.IsNullOrEmpty(project.StartupProjectPath))
                    command += $" -s \"{project.StartupProjectPath}\"";
                
                if (!string.IsNullOrEmpty(project.MigrationProjectPath))
                    command += $" -p \"{project.MigrationProjectPath}\"";
                break;

            case ProjectType.SingleLayer:
            default:
                break;
        }

        logger.LogDebug("Built add migration command: {Command}", command);
        return command;
    }

    public string BuildUpdateDatabaseCommand(ProjectProfile project, string? targetMigration)
    {
        var command = "dotnet ef database update";
        
        if (!string.IsNullOrEmpty(targetMigration))
            command += $" {targetMigration}";

        switch (project.Type)
        {
            case ProjectType.OnionArchitecture:
            case ProjectType.NTier:
                if (!string.IsNullOrEmpty(project.StartupProjectPath))
                    command += $" -s \"{project.StartupProjectPath}\"";
                
                if (!string.IsNullOrEmpty(project.MigrationProjectPath))
                    command += $" -p \"{project.MigrationProjectPath}\"";
                break;

            case ProjectType.SingleLayer:
            default:
                break;
        }

        logger.LogDebug("Built update database command: {Command}", command);
        return command;
    }

    public string GenerateNextMigrationName(string migrationFolderPath)
    {
        if (!Directory.Exists(migrationFolderPath))
        {
            logger.LogDebug("Migration folder does not exist, returning default name: mig_1");
            return "mig_1";
        }

        var files = Directory.GetFiles(migrationFolderPath, "*.cs");
        var maxNumber = 0;

        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var parts = fileName.Split('_');
            if (parts.Length > 0 && int.TryParse(parts.Last(), out var number) && number > maxNumber)
                maxNumber = number;
        }

        var nextName = $"mig_{maxNumber + 1}";
        logger.LogDebug("Generated next migration name: {MigrationName}", nextName);
        return nextName;
    }
}
    