using MediatR;

namespace Atlas.Application.Features.LeaderModals.Queries.GetLeaderModals;

public record GetLeaderModalsQuery : IRequest<List<LeaderModalDto>>;

public record LeaderModalDto(Guid Id, string ModalType, bool HasBeenSeen, DateTime? DismissedAt, string? PayloadJson, Guid? TeamId);

