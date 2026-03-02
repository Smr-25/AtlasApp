using Atlas.Application.Features.PersonalTokens.Commands.CreateToken;
using Atlas.Application.Features.PersonalTokens.Commands.RevokeToken;
using Atlas.Application.Features.PersonalTokens.Queries.GetTokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class PersonalTokensController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetTokensQuery());
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedResponse(result);
    }

    [HttpPost("{id}/revoke")]
    public async Task<IActionResult> Revoke(Guid id)
    {
        await Mediator.Send(new RevokeTokenCommand(id));
        return NoContentResponse();
    }
}

