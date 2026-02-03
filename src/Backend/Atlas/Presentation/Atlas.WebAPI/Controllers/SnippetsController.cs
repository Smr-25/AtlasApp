using Atlas.Application.Features.Snippets.Commands.CreateSnippet;
using Atlas.Application.Features.Snippets.Dtos;
using Atlas.Application.Features.Snippets.Queries.GetSnippets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SnippetsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SnippetDto>>> GetAll()
    {
        return await Mediator.Send(new GetSnippetsQuery());
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateSnippetCommand command)
    {
        return await Mediator.Send(command);
    }
}