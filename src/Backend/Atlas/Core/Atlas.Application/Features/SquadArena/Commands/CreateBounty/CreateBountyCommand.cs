using MediatR;

namespace Atlas.Application.Features.SquadArena.Commands.CreateBounty;

public record CreateBountyCommand(Guid TeamId, string Title, string? Description, int RewardPoints, string? JiraIssueKey) : IRequest<Guid>;

