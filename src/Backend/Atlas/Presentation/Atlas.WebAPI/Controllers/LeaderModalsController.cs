using Atlas.Application.Features.LeaderModals.Commands.DismissModal;
using Atlas.Application.Features.LeaderModals.Commands.OpenModal;
using Atlas.Application.Features.LeaderModals.Queries.GetLeaderModals;
using Atlas.Application.Features.LeaderModals.Queries.GetModalPayload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class LeaderModalsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetModals()
    {
        var result = await Mediator.Send(new GetLeaderModalsQuery());
        return OkResponse(result);
    }

    [HttpGet("{modalId:guid}/payload")]
    public async Task<IActionResult> GetModalPayload(Guid modalId)
    {
        var result = await Mediator.Send(new GetLeaderModalPayloadQuery(modalId));
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> OpenModal([FromBody] OpenLeaderModalCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(new { Id = id });
    }

    [HttpPost("{modalId:guid}/dismiss")]
    public async Task<IActionResult> DismissModal(Guid modalId)
    {
        await Mediator.Send(new DismissLeaderModalCommand(modalId));
        return NoContentResponse();
    }
}

