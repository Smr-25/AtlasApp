using MediatR;

namespace Atlas.Application.Features.Hotkeys.Commands.SetHotkey;

public record SetHotkeyCommand(string Action, string KeyCombination, bool IsGlobal) : IRequest<Guid>;

