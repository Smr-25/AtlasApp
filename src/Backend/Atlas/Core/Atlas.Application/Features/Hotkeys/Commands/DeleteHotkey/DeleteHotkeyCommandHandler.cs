using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Hotkeys.Commands.DeleteHotkey;

public class DeleteHotkeyCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<DeleteHotkeyCommand, bool>
{
    public async Task<bool> Handle(DeleteHotkeyCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var hotkey = await dbContext.HotkeyBindings
            .FirstOrDefaultAsync(h => h.Id == request.HotkeyId && h.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("HotkeyBinding", request.HotkeyId);

        hotkey.Delete();
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

