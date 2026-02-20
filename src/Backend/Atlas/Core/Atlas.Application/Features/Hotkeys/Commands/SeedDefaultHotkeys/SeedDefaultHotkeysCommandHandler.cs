using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Hotkeys.Commands.SeedDefaultHotkeys;

public class SeedDefaultHotkeysCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<SeedDefaultHotkeysCommand, int>
{
    private static readonly (string Action, string Key, bool IsGlobal)[] DefaultBindings =
    [
        ("SendToNotion", "Cmd+I", true),
        ("PasteFromNotion", "Cmd+Shift+I", true),
        ("ToggleFocusMode", "Cmd+Shift+F", false),
        ("QuickSnippet", "Cmd+Shift+S", false),
        ("OpenCommandPalette", "Cmd+K", false),
        ("ToggleTerminal", "Cmd+`", false),
        ("SearchWorkspace", "Cmd+P", false),
        ("NewWorkspace", "Cmd+N", false)
    ];

    public async Task<int> Handle(SeedDefaultHotkeysCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var existingActions = await dbContext.HotkeyBindings
            .Where(h => h.UserId == userId)
            .Select(h => h.Action)
            .ToListAsync(cancellationToken);

        var count = 0;

        foreach (var (action, key, isGlobal) in DefaultBindings)
        {
            if (existingActions.Contains(action)) continue;

            var hotkey = HotkeyBinding.CreateDefault(userId, action, key, isGlobal);
            await dbContext.HotkeyBindings.AddAsync(hotkey, cancellationToken);
            count++;
        }

        if (count > 0)
            await dbContext.SaveChangesAsync(cancellationToken);

        return count;
    }
}

