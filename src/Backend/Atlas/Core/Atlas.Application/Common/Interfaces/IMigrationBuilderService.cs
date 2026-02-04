using Atlas.Domain.Entities;

namespace Atlas.Application.Common.Interfaces;

public interface IMigrationBuilderService
{
    string BuildAddMigrationCommand(ProjectProfile project, string migrationName);
    string BuildUpdateDatabaseCommand(ProjectProfile project, string? targetMigration);
    string GenerateNextMigrationName(string migrationFolderPath);
}