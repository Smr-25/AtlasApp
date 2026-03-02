using Atlas.Application.Features.Support.Commands.CloseTicket;
using Atlas.Application.Features.Support.Commands.CreateTicket;
using Atlas.Application.Features.Support.Queries.GetTickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SupportController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetTicketsQuery());
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(id);
    }

    [HttpPost("{id}/close")]
    public async Task<IActionResult> Close(Guid id)
    {
        await Mediator.Send(new CloseTicketCommand(id));
        return NoContentResponse();
    }
}

