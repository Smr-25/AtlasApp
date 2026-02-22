using Atlas.Application.Features.DevUtilities.Commands.ConvertBase64;
using Atlas.Application.Features.DevUtilities.Commands.GenerateSshKey;
using Atlas.Application.Features.DevUtilities.Queries.DecodeJwt;
using Atlas.Application.Features.DevUtilities.Queries.GenerateCron;
using Atlas.Application.Features.DevUtilities.Queries.TestRegex;
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
}


