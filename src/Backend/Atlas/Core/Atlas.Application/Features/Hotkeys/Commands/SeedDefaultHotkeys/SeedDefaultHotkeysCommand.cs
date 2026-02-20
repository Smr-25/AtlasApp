using MediatR;

namespace Atlas.Application.Features.Hotkeys.Commands.SeedDefaultHotkeys;

public record SeedDefaultHotkeysCommand : IRequest<int>;

