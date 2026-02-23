
namespace Atlas.Application.Features.GlobalShortcuts.Dtos;

public record CommandPaletteItemDto(
    string Id,
    string Label,
    string Category,
    string? Icon,
    string ActionType,
    string? ActionPayload
);

public record CommandPaletteResultDto(
    List<CommandPaletteItemDto> Items,
    int TotalCount
);

