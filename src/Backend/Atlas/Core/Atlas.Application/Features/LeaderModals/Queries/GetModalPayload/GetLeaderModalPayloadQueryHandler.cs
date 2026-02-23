using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.LeaderModals.Queries.GetModalPayload;

public class GetLeaderModalPayloadQueryHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<GetLeaderModalPayloadQuery, LeaderModalPayloadResult>
{
    public async Task<LeaderModalPayloadResult> Handle(GetLeaderModalPayloadQuery request, CancellationToken cancellationToken)
    {
        var modal = await dbContext.LeaderModalStates.FirstOrDefaultAsync(m => m.Id == request.ModalId, cancellationToken);
        if (modal == null)
            return new LeaderModalPayloadResult(Guid.Empty, string.Empty, null);

        modal.MarkAsSeen();
        await dbContext.SaveChangesAsync(cancellationToken);
        return new LeaderModalPayloadResult(modal.Id, modal.ModalType.ToString(), modal.PayloadJson);
    }
}

