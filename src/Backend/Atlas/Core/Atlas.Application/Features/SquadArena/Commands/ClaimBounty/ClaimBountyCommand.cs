using MediatR;

namespace Atlas.Application.Features.SquadArena.Commands.ClaimBounty;

public record ClaimBountyCommand(Guid BountyId) : IRequest<Unit>;

