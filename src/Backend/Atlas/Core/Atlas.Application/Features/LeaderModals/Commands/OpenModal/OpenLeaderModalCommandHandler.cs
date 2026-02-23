using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.LeaderModals.Commands.OpenModal;

public class OpenLeaderModalCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser
) : IRequestHandler<OpenLeaderModalCommand, Guid>
{
    public async Task<Guid> Handle(OpenLeaderModalCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var modal = LeaderModalState.Create(userId, request.ModalType, request.TeamId, request.PayloadJson);
        dbContext.LeaderModalStates.Add(modal);
        await dbContext.SaveChangesAsync(cancellationToken);
        return modal.Id;
    }
}

