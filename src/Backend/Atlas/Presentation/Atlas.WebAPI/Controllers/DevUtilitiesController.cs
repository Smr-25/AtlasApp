using Atlas.Application.Features.DevUtilities.Commands.ConvertBase64;
using Atlas.Application.Features.DevUtilities.Commands.GenerateSshKey;
using Atlas.Application.Features.DevUtilities.Queries.DecodeJwt;
using Atlas.Application.Features.DevUtilities.Queries.GenerateCron;
using Atlas.Application.Features.DevUtilities.Queries.TestRegex;
using Atlas.Application.Features.JsonTools.Queries.FormatJson;
using Atlas.Application.Features.NetworkTools.Queries.SendHttpRequest;
using Atlas.Application.Features.SecurityTools.Queries.ScanVulnerabilities;
using Atlas.Application.Features.SystemTools.Commands.KillProcess;
using Atlas.Application.Features.SystemTools.Queries.GetPortProcess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class DevUtilitiesController : ApiControllerBase
{
    [HttpPost("decode-jwt")]
    public async Task<IActionResult> DecodeJwt([FromBody] DecodeJwtQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("test-regex")]
    public async Task<IActionResult> TestRegex([FromBody] TestRegexQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("generate-cron")]
    public async Task<IActionResult> GenerateCron([FromBody] GenerateCronQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("base64")]
    public async Task<IActionResult> ConvertBase64([FromBody] ConvertBase64Command command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Result = result });
    }

    [HttpPost("ssh-key")]
    public async Task<IActionResult> GenerateSshKey([FromBody] GenerateSshKeyCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }


    [HttpPost("json/format")]
    public async Task<IActionResult> FormatJson([FromBody] FormatJsonQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(new { result });
    }


    [HttpPost("network/send-request")]
    public async Task<IActionResult> SendHttpRequest([FromBody] SendHttpRequestQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }


    [HttpPost("security/scan-dependencies")]
    public async Task<IActionResult> ScanDependencies([FromBody] ScanVulnerabilitiesQuery query)
    {
        var result = await Mediator.Send(query);
        if (result.Count == 0 || result.All(r => r.Vulnerabilities.Count == 0))
            return OkResponse(new { message = "No vulnerabilities found! Your project is secure. 🛡️" });
        return OkResponse(result);
    }


    [HttpGet("system/check-port/{port}")]
    public async Task<IActionResult> CheckPort(int port)
    {
        var result = await Mediator.Send(new GetPortProcessQuery(port));
        if (!result.IsFound)
            return NotFoundResponse($"Port {port} is free (no process found).");
        return OkResponse(result);
    }

    [HttpDelete("system/kill-process/{pid}")]
    public async Task<IActionResult> KillProcess(int pid)
    {
        await Mediator.Send(new KillProcessCommand(pid));
        return OkResponse(new { message = $"Process {pid} has been terminated." });
    }
}
