using MediatR;

namespace Atlas.Application.Features.SquadArena.Queries.GetBountyBoard;

public record GetBountyBoardQuery(Guid TeamId) : IRequest<List<BountyBoardDto>>;

public record BountyBoardDto(Guid Id, string Title, string? Description, int RewardPoints, Guid? ClaimedByUserId, bool IsCompleted, string? JiraIssueKey);

