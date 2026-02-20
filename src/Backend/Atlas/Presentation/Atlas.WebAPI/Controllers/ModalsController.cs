using Atlas.Application.Features.Modals.Commands.DismissModal;
using Atlas.Application.Features.Modals.Queries.GetPendingModals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class ModalsController : ApiControllerBase
{
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await Mediator.Send(new GetPendingModalsQuery());
        return OkResponse(result);
    }

    [HttpPost("{modalId}/dismiss")]
    public async Task<IActionResult> Dismiss(Guid modalId)
    {
        var result = await Mediator.Send(new DismissModalCommand(modalId));
        return OkResponse(result);
    }
}

