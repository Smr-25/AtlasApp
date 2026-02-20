using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Hotkeys.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Hotkeys.Queries.GetMyHotkeys;

public class GetMyHotkeysQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetMyHotkeysQuery, List<HotkeyBindingDto>>
{
    public async Task<List<HotkeyBindingDto>> Handle(GetMyHotkeysQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var hotkeys = await dbContext.HotkeyBindings
            .Where(h => h.UserId == userId)
            .OrderBy(h => h.Action)
            .ToListAsync(cancellationToken);

        return hotkeys.Select(h => new HotkeyBindingDto(
            h.Id,
            h.Action,
            h.KeyCombination,
            h.IsGlobal,
            h.IsEnabled
        )).ToList();
    }
}

