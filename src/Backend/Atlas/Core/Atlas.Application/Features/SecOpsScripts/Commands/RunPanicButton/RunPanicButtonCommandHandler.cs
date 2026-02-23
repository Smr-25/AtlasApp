using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunPanicButton;

public class RunPanicButtonCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<RunPanicButtonCommand, string>
{
    public async Task<string> Handle(RunPanicButtonCommand request, CancellationToken cancellationToken)
    {
        var iface = request.InterfaceName ?? "en0";
        var result = await scriptRunner.ExecuteAsync(
            "sudo", $"pfctl -e -f /etc/pf.conf && pfctl -t blocklist -T add 0.0.0.0/0", ".", cancellationToken);
        return $"Panic mode activated on {iface}. {result}";
    }
}

