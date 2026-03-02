using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Hotkeys.Commands.SetHotkey;

public class SetHotkeyCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    ISubscriptionGuardService subscriptionGuard)
    : IRequestHandler<SetHotkeyCommand, Guid>
{
    public async Task<Guid> Handle(SetHotkeyCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        if (!await subscriptionGuard.HasCustomHotkeysAsync(userId, cancellationToken))
            throw new ForbiddenException("Custom hotkeys require Pro or Team subscription.");

        var existing = await dbContext.HotkeyBindings
            .FirstOrDefaultAsync(h => h.UserId == userId && h.Action == request.Action, cancellationToken);

        if (existing != null)
        {
            existing.UpdateKeyCombination(request.KeyCombination);
            await dbContext.SaveChangesAsync(cancellationToken);
            return existing.Id;
        }

        var hotkey = HotkeyBinding.Create(userId, request.Action, request.KeyCombination, request.IsGlobal);
        await dbContext.HotkeyBindings.AddAsync(hotkey, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return hotkey.Id;
    }
}
