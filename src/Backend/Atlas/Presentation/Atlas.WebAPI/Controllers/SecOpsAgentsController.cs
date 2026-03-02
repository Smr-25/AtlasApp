using Atlas.Application.Features.SecOpsAgents.Commands.DetectRoguePorts;
using Atlas.Application.Features.SecOpsAgents.Commands.ScanLeakedKeys;
using Atlas.Application.Features.SecOpsAgents.Queries.CheckVpnDrop;
using Atlas.Application.Features.SecOpsAgents.Queries.DetectSuspiciousTraffic;
using Atlas.Application.Features.SecOpsAgents.Queries.SuggestAutoPatches;
using Atlas.Application.Features.SecOpsAgents.Queries.WarnExpiringSsl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SecOpsAgentsController : ApiControllerBase
{
    [HttpPost("detect-rogue-ports")]
    public async Task<IActionResult> DetectRoguePorts()
    {
        var result = await Mediator.Send(new DetectRoguePortsCommand());
        return OkResponse(result);
    }

    [HttpPost("warn-expiring-ssl")]
    public async Task<IActionResult> WarnExpiringSsl([FromBody] WarnExpiringSslQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("detect-suspicious-traffic")]
    public async Task<IActionResult> DetectSuspiciousTraffic([FromBody] DetectSuspiciousTrafficQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("scan-leaked-keys")]
    public async Task<IActionResult> ScanLeakedKeys([FromBody] ScanLeakedKeysCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("suggest-patches")]
    public async Task<IActionResult> SuggestAutoPatches([FromBody] SuggestAutoPatchesQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpGet("vpn-status")]
    public async Task<IActionResult> CheckVpnDrop()
    {
        var result = await Mediator.Send(new CheckVpnDropQuery());
        return OkResponse(result);
    }
}
