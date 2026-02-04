using Atlas.Application.Features.Knowledge.Dtos;
using Atlas.Application.Features.Knowledge.Queries.GetNotionPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class KnowledgeController : ApiControllerBase
{
    [HttpGet("notion")]
    public async Task<ActionResult<List<NoteDto>>> GetNotionDocs()
    {
        return await Mediator.Send(new GetNotionPagesQuery());
    }
}