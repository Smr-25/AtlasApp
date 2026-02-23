using Atlas.Application.Features.Zeplin.Queries.GetScreens;
using Atlas.Application.Features.Zeplin.Queries.GetStyleGuide;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class ZeplinController : ApiControllerBase
{
    [HttpGet("{integrationId}/screens")]
    public async Task<IActionResult> GetScreens(Guid integrationId, [FromQuery] string projectId)
    {
        var result = await Mediator.Send(new GetZeplinScreensQuery(integrationId, projectId));
        return OkResponse(result);
    }

    [HttpGet("{integrationId}/style-guide")]
    public async Task<IActionResult> GetStyleGuide(Guid integrationId, [FromQuery] string projectId)
    {
        var result = await Mediator.Send(new GetZeplinStyleGuideQuery(integrationId, projectId));
        return OkResponse(result);
    }
}

