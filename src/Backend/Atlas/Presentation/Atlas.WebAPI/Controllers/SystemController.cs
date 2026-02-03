using Atlas.Application.Features.System.Dtos;
using Atlas.Application.Features.System.Queries.GetActiveIdes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

public class SystemController : ApiControllerBase
{
    [HttpGet("ides")]
    public async Task<ActionResult<List<IdeStatusDto>>> GetActiveIdes()
    {
        return await Mediator.Send(new GetActiveIdesQuery());
    }
}