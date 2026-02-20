using MediatR;

namespace Atlas.Application.Features.Hotkeys.Commands.DeleteHotkey;

public record DeleteHotkeyCommand(Guid HotkeyId) : IRequest<bool>;

