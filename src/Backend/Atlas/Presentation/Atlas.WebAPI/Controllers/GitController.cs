using Atlas.Application.Features.GitHub.Commands.ApprovePr;
using Atlas.Application.Features.GitHub.Commands.MergePr;
using Atlas.Application.Features.GitHub.Queries.GetDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class GitController : ApiControllerBase
{
    [HttpGet("dashboard/{integrationId}")]
    public async Task<ActionResult<GitDashboardVm>> GetDashboard(Guid integrationId)
    {
        return await Mediator.Send(new GetGitDashboardQuery(integrationId));
    }

    [HttpPost("approve")]
    public async Task<IActionResult> ApprovePr([FromBody] ApprovePrCommand command)
    {
        await Mediator.Send(command);
        return NoContent(); 
    }
    [HttpPost("merge")]
    public async Task<IActionResult> MergePr([FromBody] MergePrCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpPost("retry-pipeline")]
    public async Task<IActionResult> RetryPipeline([FromBody] RetryPipelineCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpPost("create-branch")]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
            ]
    }
}