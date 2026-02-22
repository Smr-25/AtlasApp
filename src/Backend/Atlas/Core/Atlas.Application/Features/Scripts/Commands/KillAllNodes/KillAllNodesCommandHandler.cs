using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.KillAllNodes;

public class KillAllNodesCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<KillAllNodesCommand, string>
{
    public async Task<string> Handle(KillAllNodesCommand request, CancellationToken cancellationToken)
    {
        var isWindows = OperatingSystem.IsWindows();
        var command = isWindows ? "taskkill" : "pkill";
        var args = isWindows ? "/F /IM node.exe" : "-f node";

        return await scriptRunner.ExecuteAsync(command, args, ".", cancellationToken);
    }
}

