using Atlas.Application.Features.AuditLogs.Queries.GetActiveSessions;
using Atlas.Application.Features.AuditLogs.Queries.GetAuditLogs;
using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class AuditLogsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLogs(
        [FromQuery] AuditAction? action,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await Mediator.Send(new GetAuditLogsQuery(action, from, to, page, pageSize));
        return OkResponse(result);
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetActiveSessions()
    {
        var result = await Mediator.Send(new GetActiveSessionsQuery());
        return OkResponse(result);
    }
}

