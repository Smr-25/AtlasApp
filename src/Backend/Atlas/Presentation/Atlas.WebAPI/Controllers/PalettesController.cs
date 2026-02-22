using Atlas.Application.Features.Design.Commands.AddColor;
using Atlas.Application.Features.Design.Commands.CreatePalette;
using Atlas.Application.Features.Design.Queries.GetUserPalettes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class PalettesController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetUserPalettesQuery());
        return OkResponse(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaletteCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(id);
    }

    [HttpPost("{id}/colors")]
    public async Task<IActionResult> AddColor(Guid id, [FromBody] AddColorCommand command)
    {
        if (id != command.PaletteId) return BadRequestResponse("ID mismatch");
        var colorId = await Mediator.Send(command);
        return CreatedResponse(colorId);
    }
}