namespace Atlas.Application.Features.Hotkeys.Dtos;

public record HotkeyBindingDto(
    Guid Id,
    string Action,
    string KeyCombination,
    bool IsGlobal,
    bool IsEnabled
);

