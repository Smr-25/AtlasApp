using Atlas.Application.Features.LottieFiles.Queries.SearchAnimations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class LottieFilesController : ApiControllerBase
{
    [HttpGet("{integrationId}/search")]
    public async Task<IActionResult> SearchAnimations(Guid integrationId, [FromQuery] string query)
    {
        var result = await Mediator.Send(new SearchLottieAnimationsQuery(integrationId, query));
        return OkResponse(result);
    }
}

