using Atlas.Application.Features.SonarQube.Queries.GetProjectQuality;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SonarQubeController : ApiControllerBase
{
    [HttpGet("{integrationId}/quality")]
    public async Task<IActionResult> GetProjectQuality(Guid integrationId, [FromQuery] string projectKey)
    {
        var result = await Mediator.Send(new GetProjectQualityQuery(integrationId, projectKey));
        return OkResponse(result);
    }
}

