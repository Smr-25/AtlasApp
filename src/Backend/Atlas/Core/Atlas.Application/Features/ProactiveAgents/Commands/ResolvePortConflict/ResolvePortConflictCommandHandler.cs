using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Commands.ResolvePortConflict;

public class ResolvePortConflictCommandHandler(
    ISystemToolAdapter systemTool
) : IRequestHandler<ResolvePortConflictCommand, string>
{
    public async Task<string> Handle(ResolvePortConflictCommand request, CancellationToken cancellationToken)
    {
        var process = await systemTool.GetProcessByPortAsync(request.Port, cancellationToken);
        if (!process.IsFound)
            return $"Port {request.Port} is free.";

        await systemTool.KillProcessAsync(process.Pid, cancellationToken);
        return $"Process {process.Name} (PID: {process.Pid}) on port {request.Port} has been killed.";
    }
}


