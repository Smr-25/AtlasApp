using Atlas.Application.Features.Snippets.Commands.CreateSnippet;
using Atlas.Application.Features.Snippets.Commands.DeleteSnippet;
using Atlas.Application.Features.Snippets.Commands.PasteFromNotion;
using Atlas.Application.Features.Snippets.Commands.SendSnippetToNotion;
using Atlas.Application.Features.Snippets.Commands.ToggleFavorite;
using Atlas.Application.Features.Snippets.Commands.UpdateSnippet;
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

    [HttpPut("{snippetId}")]
    public async Task<IActionResult> Update(Guid snippetId, [FromBody] UpdateSnippetCommand command)
    {
        if (snippetId != command.SnippetId) return BadRequestResponse("Snippet ID mismatch.");
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpDelete("{snippetId}")]
    public async Task<IActionResult> Delete(Guid snippetId)
    {
        var result = await Mediator.Send(new DeleteSnippetCommand(snippetId));
        return OkResponse(result);
    }

    [HttpPatch("{snippetId}/favorite")]
    public async Task<IActionResult> ToggleFavorite(Guid snippetId)
    {
        var result = await Mediator.Send(new ToggleSnippetFavoriteCommand(snippetId));
        return OkResponse(new { IsFavorite = result });
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