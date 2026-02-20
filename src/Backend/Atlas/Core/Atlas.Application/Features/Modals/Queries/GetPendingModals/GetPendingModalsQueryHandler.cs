using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Modals.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Modals.Queries.GetPendingModals;

public class GetPendingModalsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetPendingModalsQuery, List<ModalStateDto>>
{
    public async Task<List<ModalStateDto>> Handle(GetPendingModalsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var modals = await dbContext.ModalStates
            .Where(m => m.UserId == userId && !m.HasBeenSeen)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        return modals.Select(m => new ModalStateDto(
            m.Id,
            m.ModalType.ToString(),
            m.HasBeenSeen,
            m.PayloadJson,
            m.CreatedAt
        )).ToList();
    }
}

