using Atlas.Application.Features.SquadArena.Commands.AwardBadge;
using Atlas.Application.Features.SquadArena.Commands.ClaimBounty;
using Atlas.Application.Features.SquadArena.Commands.CompleteBounty;
using Atlas.Application.Features.SquadArena.Commands.CreateBounty;
using Atlas.Application.Features.SquadArena.Queries.GetBountyBoard;
using Atlas.Application.Features.SquadArena.Queries.GetLeaderboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SquadArenaController : ApiControllerBase
{
    [HttpGet("leaderboard/{teamId:guid}")]
    public async Task<IActionResult> GetLeaderboard(Guid teamId)
    {
        var result = await Mediator.Send(new GetLeaderboardQuery(teamId));
        return OkResponse(result);
    }

    [HttpGet("bounties/{teamId:guid}")]
    public async Task<IActionResult> GetBountyBoard(Guid teamId)
    {
        var result = await Mediator.Send(new GetBountyBoardQuery(teamId));
        return OkResponse(result);
    }

    [HttpPost("badge")]
    public async Task<IActionResult> AwardBadge([FromBody] AwardBadgeCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(new { Id = id });
    }

    [HttpPost("bounty")]
    public async Task<IActionResult> CreateBounty([FromBody] CreateBountyCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(new { Id = id });
    }

    [HttpPost("bounty/{bountyId:guid}/claim")]
    public async Task<IActionResult> ClaimBounty(Guid bountyId)
    {
        await Mediator.Send(new ClaimBountyCommand(bountyId));
        return NoContentResponse();
    }

    [HttpPost("bounty/{bountyId:guid}/complete")]
    public async Task<IActionResult> CompleteBounty(Guid bountyId)
    {
        await Mediator.Send(new CompleteBountyCommand(bountyId));
        return NoContentResponse();
    }
}

