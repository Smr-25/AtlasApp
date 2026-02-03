namespace Atlas.Application.Features.System.Dtos;

public record IdeStatusDto(
    string Name,
    string ProcessName,
    double MemoryUsageMb,
    TimeSpan Uptime,
    bool IsResponding
);