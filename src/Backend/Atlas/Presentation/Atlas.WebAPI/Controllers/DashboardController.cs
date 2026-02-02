using Atlas.Application.Features.Dashboard.Queries.GetDashboardStats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class DashboardController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var result = await Mediator.Send(new GetDashboardStatsQuery());
        return OkResponse(result);
    }
}