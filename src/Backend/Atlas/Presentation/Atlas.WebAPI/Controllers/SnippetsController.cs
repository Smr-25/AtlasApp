using Atlas.Application.Features.Snippets.Commands.CreateSnippet;
using Atlas.Application.Features.Snippets.Commands.PasteFromNotion;
using Atlas.Application.Features.Snippets.Commands.SendSnippetToNotion;
using Atlas.Application.Features.Snippets.Dtos;
using Atlas.Application.Features.Snippets.Queries.GetSnippets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SnippetsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetSnippetsQuery());
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSnippetCommand command)
    {
        var snippetId = await Mediator.Send(command);
        return CreatedResponse(snippetId);
    }

    [HttpPost("send-to-notion")]
    public async Task<IActionResult> SendToNotion([FromBody] SendSnippetToNotionCommand command)
    {
        var notionPageId = await Mediator.Send(command);
        return OkResponse(new { NotionPageId = notionPageId });
    }

    [HttpPost("paste-from-notion")]
    public async Task<IActionResult> PasteFromNotion([FromBody] PasteFromNotionCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
}