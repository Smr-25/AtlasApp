using MediatR;

namespace Atlas.Application.Features.SquadArena.Commands.CompleteBounty;

public record CompleteBountyCommand(Guid BountyId) : IRequest<Unit>;

