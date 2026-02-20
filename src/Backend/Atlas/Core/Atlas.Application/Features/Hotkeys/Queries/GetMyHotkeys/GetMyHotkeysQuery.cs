using Atlas.Application.Features.Hotkeys.Dtos;
using MediatR;

namespace Atlas.Application.Features.Hotkeys.Queries.GetMyHotkeys;

public record GetMyHotkeysQuery : IRequest<List<HotkeyBindingDto>>;

