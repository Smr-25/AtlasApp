using Atlas.Application.Features.Sentry.Commands.ResolveSentryIssue;
using Atlas.Application.Features.Sentry.Queries.GetSentryIssueDetail;
using Atlas.Application.Features.Sentry.Queries.GetSentryIssues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SentryController : ApiControllerBase
{
    [HttpGet("{integrationId}/issues")]
    public async Task<IActionResult> GetIssues(Guid integrationId, [FromQuery] string projectSlug)
    {
        var result = await Mediator.Send(new GetSentryIssuesQuery(integrationId, projectSlug));
        return OkResponse(result);
    }

    [HttpGet("{integrationId}/issues/{issueId}")]
    public async Task<IActionResult> GetIssueDetail(Guid integrationId, string issueId)
    {
        var result = await Mediator.Send(new GetSentryIssueDetailQuery(integrationId, issueId));
        return OkResponse(result);
    }

    [HttpPost("issues/{issueId}/resolve")]
    public async Task<IActionResult> ResolveIssue([FromBody] ResolveSentryIssueCommand command)
    {
        await Mediator.Send(command);
        return NoContentResponse();
    }
}

