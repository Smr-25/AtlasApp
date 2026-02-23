using Atlas.Application.Features.MarketerUtilities.Commands.GenerateCopywriting;
using Atlas.Application.Features.MarketerUtilities.Queries.AnalyzeKeywordDensity;
using Atlas.Application.Features.MarketerUtilities.Queries.CalculateReadability;
using Atlas.Application.Features.MarketerUtilities.Queries.CheckSeoMeta;
using Atlas.Application.Features.MarketerUtilities.Queries.ConvertMarkdownToHtml;
using Atlas.Application.Features.MarketerUtilities.Queries.SearchEmojis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class MarketerUtilitiesController : ApiControllerBase
{
    [HttpPost("seo-check")]
    public async Task<IActionResult> CheckSeoMeta([FromBody] CheckSeoMetaQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("copywriting")]
    public async Task<IActionResult> GenerateCopywriting([FromBody] GenerateCopywritingCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Copy = result });
    }

    [HttpPost("markdown-to-html")]
    public async Task<IActionResult> ConvertMarkdownToHtml([FromBody] ConvertMarkdownToHtmlQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(new { Html = result });
    }

    [HttpPost("keyword-density")]
    public async Task<IActionResult> AnalyzeKeywordDensity([FromBody] AnalyzeKeywordDensityQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("readability")]
    public async Task<IActionResult> CalculateReadability([FromBody] CalculateReadabilityQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("emojis")]
    public async Task<IActionResult> SearchEmojis([FromBody] SearchEmojisQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }
}

