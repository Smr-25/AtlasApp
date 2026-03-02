using Atlas.Application.Features.Search.Queries.GlobalSearch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SearchController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 5)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return OkResponse(new { });

        var result = await Mediator.Send(new GlobalSearchQuery(q, limit));
        return OkResponse(result);
    }
}

