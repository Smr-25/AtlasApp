using Atlas.Application.Features.Dribbble.Queries.GetInspiration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class DribbbleController : ApiControllerBase
{
    [HttpGet("{integrationId}/inspiration")]
    public async Task<IActionResult> GetInspiration(Guid integrationId, [FromQuery] string? query)
    {
        var result = await Mediator.Send(new GetDribbbleInspirationQuery(integrationId, query));
        return OkResponse(result);
    }
}

