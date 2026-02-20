using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Modals.Commands.DismissModal;

public class DismissModalCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<DismissModalCommand, bool>
{
    public async Task<bool> Handle(DismissModalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var modal = await dbContext.ModalStates
            .FirstOrDefaultAsync(m => m.Id == request.ModalId && m.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("ModalState", request.ModalId);

        modal.Dismiss();
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

