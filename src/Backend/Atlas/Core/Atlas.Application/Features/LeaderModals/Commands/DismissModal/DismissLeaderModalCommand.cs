using MediatR;

namespace Atlas.Application.Features.LeaderModals.Commands.DismissModal;

public record DismissLeaderModalCommand(Guid ModalId) : IRequest<Unit>;

