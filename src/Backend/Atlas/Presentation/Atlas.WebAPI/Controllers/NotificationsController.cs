using Atlas.Application.Features.Notifications.Commands.DeleteNotification;
using Atlas.Application.Features.Notifications.Commands.ExecuteAction;
using Atlas.Application.Features.Notifications.Commands.MarkAllAsRead;
using Atlas.Application.Features.Notifications.Commands.MarkAsRead;
using Atlas.Application.Features.Notifications.Queries.GetNotifications;
using Atlas.Application.Features.Notifications.Queries.GetUnreadCount;
using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class NotificationsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] NotificationCategory? category,
        [FromQuery] bool? unreadOnly,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        var result = await Mediator.Send(new GetNotificationsQuery(category, unreadOnly, page, pageSize));
        return OkResponse(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var result = await Mediator.Send(new GetUnreadCountQuery());
        return OkResponse(result);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await Mediator.Send(new MarkAsReadCommand(id));
        return NoContentResponse();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead([FromQuery] NotificationCategory? category)
    {
        var count = await Mediator.Send(new MarkAllAsReadCommand(category));
        return OkResponse(new { MarkedAsRead = count });
    }

    [HttpPost("{id}/execute")]
    public async Task<IActionResult> ExecuteAction(Guid id)
    {
        var payload = await Mediator.Send(new ExecuteNotificationActionCommand(id));
        return OkResponse(new { ActionPayload = payload });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteNotificationCommand(id));
        return NoContentResponse();
    }
}

