using Atlas.Application.Features.LeaderUtilities.Commands.CreateDecisionLog;
using Atlas.Application.Features.LeaderUtilities.Commands.GenerateQuickPoll;
using Atlas.Application.Features.LeaderUtilities.Commands.GenerateRiskMatrix;
using Atlas.Application.Features.LeaderUtilities.Commands.RenderMarkdown;
using Atlas.Application.Features.LeaderUtilities.Queries.CalculateCapacity;
using Atlas.Application.Features.LeaderUtilities.Queries.ConvertTimezones;
using Atlas.Application.Features.LeaderUtilities.Queries.EstimateCost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class LeaderUtilitiesController : ApiControllerBase
{
    [HttpPost("timezones")]
    public async Task<IActionResult> ConvertTimezones([FromBody] ConvertTimezonesQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("quick-poll")]
    public async Task<IActionResult> GenerateQuickPoll([FromBody] GenerateQuickPollCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("capacity")]
    public async Task<IActionResult> CalculateCapacity([FromBody] CalculateCapacityQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("cost-estimate")]
    public async Task<IActionResult> EstimateCost([FromBody] EstimateCostQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("risk-matrix")]
    public async Task<IActionResult> GenerateRiskMatrix([FromBody] GenerateRiskMatrixCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("decision-log")]
    public async Task<IActionResult> CreateDecisionLog([FromBody] CreateDecisionLogCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("markdown")]
    public async Task<IActionResult> RenderMarkdown([FromBody] RenderMarkdownCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Html = result });
    }
}

