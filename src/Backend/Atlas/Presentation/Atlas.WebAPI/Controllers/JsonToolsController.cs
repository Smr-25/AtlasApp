using Atlas.Application.Features.JsonTools.Queries.FormatJson;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JsonToolsController : ApiControllerBase
{
    [HttpPost("format")]
    public async Task<IActionResult> Format([FromBody] FormatJsonQuery query)
    {
        try
        {
            var result = await Mediator.Send(query);
            return OkResponse(new { result }); 
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }
}