using MediatR;

namespace Atlas.Application.Features.Modals.Commands.DismissModal;

public record DismissModalCommand(Guid ModalId) : IRequest<bool>;

