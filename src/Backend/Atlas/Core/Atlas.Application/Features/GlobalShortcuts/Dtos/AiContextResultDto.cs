namespace Atlas.Application.Features.GlobalShortcuts.Dtos;

public record AiContextResultDto(
    string Action,
    string OriginalContent,
    string ProcessedContent,
    string? DetectedRole
);

