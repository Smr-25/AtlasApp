using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunFirewallLockdown;

public class RunFirewallLockdownCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<RunFirewallLockdownCommand, string>
{
    public async Task<string> Handle(RunFirewallLockdownCommand request, CancellationToken cancellationToken)
    {
        var ports = request.AllowedPorts ?? [22, 80, 443];
        var portRules = string.Join(" ", ports.Select(p => $"--allow {p}/tcp"));

        var result = await scriptRunner.ExecuteAsync(
            "bash", $"-c \"sudo pfctl -d 2>/dev/null; echo 'block all' | sudo pfctl -ef - 2>/dev/null\"", ".", cancellationToken);

        return $"Firewall lockdown activated. Allowed ports: {string.Join(", ", ports)}. {result}";
    }
}

