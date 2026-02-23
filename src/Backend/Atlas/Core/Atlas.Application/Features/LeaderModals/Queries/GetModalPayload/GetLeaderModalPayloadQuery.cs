using MediatR;

namespace Atlas.Application.Features.LeaderModals.Queries.GetModalPayload;

public record GetLeaderModalPayloadQuery(Guid ModalId) : IRequest<LeaderModalPayloadResult>;

public record LeaderModalPayloadResult(Guid Id, string ModalType, string? PayloadJson);

