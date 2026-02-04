using Atlas.Application.Features.System.Dtos;
using Atlas.Application.Features.System.Queries.GetActiveIdes;
using Atlas.Application.Features.System.Queries.GetAiAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SystemController : ApiControllerBase
{
    [HttpGet("ides")]
    public async Task<IActionResult> GetActiveIdes()
    {
        var result = await Mediator.Send(new GetActiveIdesQuery());
        return OkResponse(result);
    }
    
    [HttpGet("analyze")]
    public async Task<IActionResult> Analyze()
    {
        var result = await Mediator.Send(new GetAiAnalysisQuery());
        return OkResponse(result);
    }
}