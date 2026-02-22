using Atlas.Application.Features.Perplexity.Queries.SearchError;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class PerplexityController : ApiControllerBase
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchError([FromBody] SearchErrorQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }
}

