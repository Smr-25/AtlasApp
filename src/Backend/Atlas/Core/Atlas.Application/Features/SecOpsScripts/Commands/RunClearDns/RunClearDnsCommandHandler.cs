using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunClearDns;

public class RunClearDnsCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<RunClearDnsCommand, string>
{
    public async Task<string> Handle(RunClearDnsCommand request, CancellationToken cancellationToken)
    {
        var result = await scriptRunner.ExecuteAsync(
            "sudo", "dscacheutil -flushcache && sudo killall -HUP mDNSResponder", ".", cancellationToken);
        return $"DNS cache cleared. {result}";
    }
}

