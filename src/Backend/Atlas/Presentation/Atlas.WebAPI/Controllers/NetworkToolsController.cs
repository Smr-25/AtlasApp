using Atlas.Application.Features.NetworkTools.Queries.SendHttpRequest;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
public class NetworkToolsController : ApiControllerBase
{
    [HttpPost("send-request")]
    public async Task<IActionResult> SendRequest([FromBody] SendHttpRequestQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }
}