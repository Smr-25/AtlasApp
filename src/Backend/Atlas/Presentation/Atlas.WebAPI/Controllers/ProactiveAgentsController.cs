using Atlas.Application.Features.Perplexity.Queries.SearchError;
using Atlas.Application.Features.ProactiveAgents.Commands.ExplainError;
using Atlas.Application.Features.ProactiveAgents.Commands.KillIdleContainers;
using Atlas.Application.Features.ProactiveAgents.Commands.ResolvePortConflict;
using Atlas.Application.Features.ProactiveAgents.Queries.SuggestCommitMessage;
using Atlas.Application.Features.ProactiveAgents.Queries.SummarizePr;
using Atlas.Application.Features.ProactiveAgents.Queries.WatchDependencies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class ProactiveAgentsController : ApiControllerBase
{
    [HttpPost("explain-error")]
    public async Task<IActionResult> ExplainError([FromBody] ExplainErrorCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Explanation = result });
    }

    [HttpPost("resolve-port")]
    public async Task<IActionResult> ResolvePortConflict([FromBody] ResolvePortConflictCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Result = result });
    }

    [HttpPost("kill-idle-containers")]
    public async Task<IActionResult> KillIdleContainers([FromBody] KillIdleContainersCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Result = result });
    }

    [HttpPost("suggest-commit")]
    public async Task<IActionResult> SuggestCommitMessage([FromBody] SuggestCommitMessageQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(new { Message = result });
    }

    [HttpPost("summarize-pr")]
    public async Task<IActionResult> SummarizePr([FromBody] SummarizePrQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(new { Summary = result });
    }

    [HttpPost("watch-dependencies")]
    public async Task<IActionResult> WatchDependencies([FromBody] WatchDependenciesQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }
    
    [HttpPost("search")]
    public async Task<IActionResult> SearchError([FromBody] SearchErrorQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }
}
