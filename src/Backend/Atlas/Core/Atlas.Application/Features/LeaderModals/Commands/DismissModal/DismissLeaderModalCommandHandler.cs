using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.LeaderModals.Commands.DismissModal;

public class DismissLeaderModalCommandHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<DismissLeaderModalCommand, Unit>
{
    public async Task<Unit> Handle(DismissLeaderModalCommand request, CancellationToken cancellationToken)
    {
        var modal = await dbContext.LeaderModalStates.FirstOrDefaultAsync(m => m.Id == request.ModalId, cancellationToken);
        if (modal != null)
        {
            modal.Dismiss();
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}

