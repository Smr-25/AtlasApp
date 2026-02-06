namespace Atlas.Application.Features.Projects.Dtos;

public record LocalProjectDto(
    Guid Id,
    string Name,
    string Path,
    string? Directory,
    string Type,
    bool HasEfCore,
    string TargetFramework
);