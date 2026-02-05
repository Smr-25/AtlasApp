using Atlas.Application.Features.Design.Commands.ConvertAsset;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class DesignController : ApiControllerBase
{
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertAsset([FromForm] ConvertAssetCommand command)
    {
        var result = await Mediator.Send(command);
        return File(result.FileStream, result.ContentType, result.FileName);
    }
}