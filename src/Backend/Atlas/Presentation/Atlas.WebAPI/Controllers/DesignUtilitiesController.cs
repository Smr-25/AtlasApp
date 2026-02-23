using Atlas.Application.Features.Design.Commands.CompressImage;
using Atlas.Application.Features.Design.Commands.ExtractCssVars;
using Atlas.Application.Features.Design.Commands.OptimizeSvg;
using Atlas.Application.Features.DesignUtilities.Queries.CalculateAspectRatio;
using Atlas.Application.Features.DesignUtilities.Queries.CheckContrast;
using Atlas.Application.Features.DesignUtilities.Queries.GenerateDummyData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class DesignUtilitiesController : ApiControllerBase
{
    [HttpPost("compress-image")]
    public async Task<IActionResult> CompressImage([FromBody] CompressImageCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("extract-css")]
    public async Task<IActionResult> ExtractCssVars([FromBody] ExtractCssVarsCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Css = result });
    }

    [HttpPost("optimize-svg")]
    public async Task<IActionResult> OptimizeSvg([FromBody] OptimizeSvgCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("check-contrast")]
    public async Task<IActionResult> CheckContrast([FromBody] CheckContrastQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpGet("aspect-ratio")]
    public async Task<IActionResult> CalculateAspectRatio([FromQuery] int width, [FromQuery] int height)
    {
        var result = await Mediator.Send(new CalculateAspectRatioQuery(width, height));
        return OkResponse(result);
    }

    [HttpGet("dummy-data")]
    public async Task<IActionResult> GenerateDummyData([FromQuery] string type = "user", [FromQuery] int count = 10)
    {
        var result = await Mediator.Send(new GenerateDummyDataQuery(type, count));
        return OkResponse(result);
    }
}

