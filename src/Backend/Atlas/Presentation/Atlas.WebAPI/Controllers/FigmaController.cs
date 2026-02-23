using Atlas.Application.Features.Figma.Commands.ResolveComment;
using Atlas.Application.Features.Figma.Queries.GetComments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class FigmaController : ApiControllerBase
{
    [HttpGet("{integrationId}/comments")]
    public async Task<IActionResult> GetComments(Guid integrationId, [FromQuery] string fileKey)
    {
        var result = await Mediator.Send(new GetFigmaCommentsQuery(integrationId, fileKey));
        return OkResponse(result);
    }

    [HttpPost("comments/resolve")]
    public async Task<IActionResult> ResolveComment([FromBody] ResolveFigmaCommentCommand command)
    {
        await Mediator.Send(command);
        return NoContentResponse();
    }
}

