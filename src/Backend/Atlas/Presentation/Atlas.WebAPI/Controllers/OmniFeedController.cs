using Atlas.Application.Features.OmniFeed.Commands.AddEmojiReaction;
using Atlas.Application.Features.OmniFeed.Commands.MarkFeedItemRead;
using Atlas.Application.Features.OmniFeed.Commands.PublishManualItem;
using Atlas.Application.Features.OmniFeed.Queries.GetOmniFeed;
using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class OmniFeedController : ApiControllerBase
{
    [HttpGet("{teamId:guid}")]
    public async Task<IActionResult> GetFeed(Guid teamId, [FromQuery] OmniFeedSource? source, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetOmniFeedQuery(teamId, source, page, pageSize));
        return OkResponse(result);
    }

    [HttpPost("publish")]
    public async Task<IActionResult> PublishManualItem([FromBody] PublishManualItemCommand command)
    {
        await Mediator.Send(command);
        return NoContentResponse();
    }

    [HttpPost("{itemId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid itemId)
    {
        await Mediator.Send(new MarkFeedItemReadCommand(itemId));
        return NoContentResponse();
    }

    [HttpPost("{itemId:guid}/emoji")]
    public async Task<IActionResult> AddEmoji(Guid itemId, [FromBody] AddEmojiReactionCommand command)
    {
        await Mediator.Send(command with { ItemId = itemId });
        return NoContentResponse();
    }
}

