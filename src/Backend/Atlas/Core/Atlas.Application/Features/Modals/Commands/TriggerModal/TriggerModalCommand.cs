using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Modals.Commands.TriggerModal;

public record TriggerModalCommand(Guid UserId, ModalType ModalType, string? PayloadJson = null) : IRequest<Guid>;

