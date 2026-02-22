using Atlas.Application.Features.SystemTools.Commands.KillProcess;
using Atlas.Application.Features.SystemTools.Queries.GetPortProcess;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SystemToolsController : ApiControllerBase 
{
    [HttpGet("check-port/{port}")]
    public async Task<IActionResult> CheckPort(int port)
    {
        var result = await Mediator.Send(new GetPortProcessQuery(port));
        
        if (!result.IsFound)
            return NotFoundResponse($"Port {port} is free (no process found).");

        return OkResponse(result);
    }

    [HttpDelete("kill-process/{pid}")]
    public async Task<IActionResult> KillProcess(int pid)
    {
        try
        {
            await Mediator.Send(new KillProcessCommand(pid));
            return OkResponse(new { message = $"Process {pid} has been terminated." });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }
}