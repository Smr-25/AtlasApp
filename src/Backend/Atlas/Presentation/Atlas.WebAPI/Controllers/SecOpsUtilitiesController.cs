using Atlas.Application.Features.SecOpsUtilities.Commands.EncodePayload;
using Atlas.Application.Features.SecOpsUtilities.Commands.GenerateHash;
using Atlas.Application.Features.SecOpsUtilities.Queries.CalculatePasswordEntropy;
using Atlas.Application.Features.SecOpsUtilities.Queries.CheckSsl;
using Atlas.Application.Features.SecOpsUtilities.Queries.IpDnsLookup;
using Atlas.Application.Features.SecOpsUtilities.Queries.ScanLocalPorts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SecOpsUtilitiesController : ApiControllerBase
{
    [HttpPost("hash")]
    public async Task<IActionResult> GenerateHash([FromBody] GenerateHashCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Hash = result });
    }

    [HttpPost("ip-dns")]
    public async Task<IActionResult> IpDnsLookup([FromBody] IpDnsLookupQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("encode-payload")]
    public async Task<IActionResult> EncodePayload([FromBody] EncodePayloadCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Encoded = result });
    }

    [HttpPost("password-entropy")]
    public async Task<IActionResult> CalculatePasswordEntropy([FromBody] CalculatePasswordEntropyQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("ssl-check")]
    public async Task<IActionResult> CheckSsl([FromBody] CheckSslQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("port-scan")]
    public async Task<IActionResult> ScanLocalPorts([FromBody] ScanLocalPortsQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }
}
