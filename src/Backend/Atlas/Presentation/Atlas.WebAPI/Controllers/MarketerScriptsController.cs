using Atlas.Application.Features.MarketerScripts.Commands.RunBulkEmailVerifier;
using Atlas.Application.Features.MarketerScripts.Commands.RunCampaignPauser;
using Atlas.Application.Features.MarketerScripts.Commands.RunClearBrowserCookie;
using Atlas.Application.Features.MarketerScripts.Commands.RunCompetitorScraper;
using Atlas.Application.Features.MarketerScripts.Commands.RunSocialBlast;
using Atlas.Application.Features.MarketerScripts.Commands.RunUtmLinkSaver;
using Atlas.Application.Features.MarketerScripts.Commands.RunWeeklyReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class MarketerScriptsController : ApiControllerBase
{
    [HttpPost("pause-campaigns")]
    public async Task<IActionResult> PauseCampaigns([FromBody] RunCampaignPauserCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("social-blast")]
    public async Task<IActionResult> SocialBlast([FromBody] RunSocialBlastCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("weekly-report")]
    public async Task<IActionResult> WeeklyReport([FromBody] RunWeeklyReportCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Report = result });
    }

    [HttpPost("utm-link")]
    public async Task<IActionResult> UtmLinkSaver([FromBody] RunUtmLinkSaverCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { UtmUrl = result });
    }

    [HttpPost("competitor-scrape")]
    public async Task<IActionResult> CompetitorScraper([FromBody] RunCompetitorScraperCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("clear-cookies")]
    public async Task<IActionResult> ClearBrowserCookie([FromBody] RunClearBrowserCookieCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("verify-emails")]
    public async Task<IActionResult> BulkEmailVerifier([FromBody] RunBulkEmailVerifierCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
}

