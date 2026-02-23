using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunQuickScan;

public class RunQuickScanCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<RunQuickScanCommand, string>
{
    public async Task<string> Handle(RunQuickScanCommand request, CancellationToken cancellationToken)
    {
        var result = await scriptRunner.ExecuteAsync(
            "arp", $"-a", ".", cancellationToken);
        return result;
    }
}

