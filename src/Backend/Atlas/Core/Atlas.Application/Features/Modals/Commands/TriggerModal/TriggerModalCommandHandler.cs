using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Modals.Commands.TriggerModal;

public class TriggerModalCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<TriggerModalCommand, Guid>
{
    public async Task<Guid> Handle(TriggerModalCommand request, CancellationToken cancellationToken)
    {
        var modal = ModalState.Create(request.UserId, request.ModalType, request.PayloadJson);

        await dbContext.ModalStates.AddAsync(modal, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return modal.Id;
    }
}

