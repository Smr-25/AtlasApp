using Atlas.Application.Features.Communication.Dtos;
using Atlas.Application.Features.Communication.Queries.GetUnreadEmails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class GmailController : ApiControllerBase
{
    [HttpGet("unread")]
    public async Task<ActionResult<List<EmailDto>>> GetUnread()
    {
        return await Mediator.Send(new GetUnreadEmailsQuery());
    }
}