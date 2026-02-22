using Atlas.Application.Features.Scripts.Commands.CreateScript;
using Atlas.Application.Features.Scripts.Commands.FlushCache;
using Atlas.Application.Features.Scripts.Commands.FormatAndLint;
using Atlas.Application.Features.Scripts.Commands.GenerateBoilerplate;
using Atlas.Application.Features.Scripts.Commands.KillAllNodes;
using Atlas.Application.Features.Scripts.Commands.NukeAndMigrate;
using Atlas.Application.Features.Scripts.Commands.ResolveGitConflicts;
using Atlas.Application.Features.Scripts.Commands.RunScript;
using Atlas.Application.Features.Scripts.Commands.SpinEnvironment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class ScriptsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScriptCommand command)
    {
        var scriptId = await Mediator.Send(command);
        return CreatedResponse(scriptId);
    }
    
    [HttpPost("{id}/run")]
    public async Task<IActionResult> Run(Guid id)
    {
        var result = await Mediator.Send(new RunScriptCommand(id));
        return OkResponse(new { Output = result });
    }

    [HttpPost("spin-environment")]
    public async Task<IActionResult> SpinEnvironment([FromBody] SpinEnvironmentCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("resolve-conflicts")]
    public async Task<IActionResult> ResolveGitConflicts([FromBody] ResolveGitConflictsCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("nuke-migrate")]
    public async Task<IActionResult> NukeAndMigrate([FromBody] NukeAndMigrateCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("flush-cache")]
    public async Task<IActionResult> FlushCache([FromBody] FlushCacheCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("format-lint")]
    public async Task<IActionResult> FormatAndLint([FromBody] FormatAndLintCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("kill-nodes")]
    public async Task<IActionResult> KillAllNodes()
    {
        var result = await Mediator.Send(new KillAllNodesCommand());
        return OkResponse(new { Output = result });
    }

    [HttpPost("generate-boilerplate")]
    public async Task<IActionResult> GenerateBoilerplate([FromBody] GenerateBoilerplateCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }
}