using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;

namespace Atlas.Infrastructure.Services;

public class MigrationBuilderService : IMigrationBuilderService
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

        return command;
    }

    public string BuildUpdateDatabaseCommand(ProjectProfile project,string? targetMigration)
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

        return command;
    }

    public string GenerateNextMigrationName(string migrationFolderPath)
    {
        if (!Directory.Exists(migrationFolderPath)) return "mig_1";

        var files = Directory.GetFiles(migrationFolderPath, "*.cs");

        var maxNumber = 0;

        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            
            var parts = fileName.Split('_');
            if (parts.Length <= 0 || !int.TryParse(parts.Last(), out var number)) continue;
            if (number > maxNumber) maxNumber = number;
        }

        return $"mig_{maxNumber + 1}";
    }
}
    