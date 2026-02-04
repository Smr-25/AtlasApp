using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class ProjectProfile : BaseEntity
{
    public string Name { get; set; } = null!;           
    public ProjectType Type { get; set; }               
    public string RootPath { get; set; } = null!;       
    public string? StartupProjectPath { get; set; }     
    public string? MigrationProjectPath { get; set; }  
    public string LastMigrationName { get; set; } = ""; 
    public Guid UserId { get; set; }
    
    public static ProjectProfile Create(string name, ProjectType type, string rootPath, string? startupProjectPath, string? migrationProjectPath, Guid userId)
    {
        return new ProjectProfile
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            RootPath = rootPath,
            StartupProjectPath = startupProjectPath,
            MigrationProjectPath = migrationProjectPath,
            UserId = userId
        };
    }
}