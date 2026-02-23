using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.LeaderModals.Commands.OpenModal;

public record OpenLeaderModalCommand(LeaderModalType ModalType, Guid? TeamId, string? PayloadJson) : IRequest<Guid>;

