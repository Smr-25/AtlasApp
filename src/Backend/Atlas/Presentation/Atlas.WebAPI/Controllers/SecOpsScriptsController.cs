using Atlas.Application.Features.SecOpsScripts.Commands.RunClearDns;
using Atlas.Application.Features.SecOpsScripts.Commands.RunPhishingAlert;
using Atlas.Application.Features.SecOpsScripts.Commands.RunQuickScan;
using Atlas.Application.Features.SecOpsScripts.Commands.RunRotateSsh;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SecOpsScriptsController : ApiControllerBase
{
    [HttpPost("quick-scan")]
    public async Task<IActionResult> QuickScan([FromBody] RunQuickScanCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("phishing-alert")]
    public async Task<IActionResult> PhishingAlert([FromBody] RunPhishingAlertCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("rotate-ssh")]
    public async Task<IActionResult> RotateSsh([FromBody] RunRotateSshCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("clear-dns")]
    public async Task<IActionResult> ClearDns()
    {
        var result = await Mediator.Send(new RunClearDnsCommand());
        return OkResponse(new { Output = result });
    }
}
