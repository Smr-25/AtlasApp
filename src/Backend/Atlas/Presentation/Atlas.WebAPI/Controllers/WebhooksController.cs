using Atlas.Application.Features.Webhooks.Commands.CreateWebhook;
using Atlas.Application.Features.Webhooks.Commands.DeleteWebhook;
using Atlas.Application.Features.Webhooks.Commands.ToggleWebhook;
using Atlas.Application.Features.Webhooks.Commands.UpdateWebhook;
using Atlas.Application.Features.Webhooks.Queries.GetWebhooks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class WebhooksController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetWebhooksQuery());
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWebhookCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWebhookCommand command)
    {
        if (id != command.WebhookId) return BadRequestResponse("ID mismatch");
        await Mediator.Send(command);
        return NoContentResponse();
    }

    [HttpPost("{id}/toggle")]
    public async Task<IActionResult> Toggle(Guid id, [FromBody] ToggleWebhookRequest request)
    {
        await Mediator.Send(new ToggleWebhookCommand(id, request.Active));
        return NoContentResponse();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteWebhookCommand(id));
        return NoContentResponse();
    }
}

public record ToggleWebhookRequest(bool Active);

