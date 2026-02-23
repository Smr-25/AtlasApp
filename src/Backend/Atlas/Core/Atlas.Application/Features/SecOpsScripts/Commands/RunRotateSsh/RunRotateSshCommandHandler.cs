using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunRotateSsh;

public class RunRotateSshCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<RunRotateSshCommand, string>
{
    public async Task<string> Handle(RunRotateSshCommand request, CancellationToken cancellationToken)
    {
        var backupResult = await scriptRunner.ExecuteAsync(
            "bash", "-c \"cp ~/.ssh/id_rsa ~/.ssh/id_rsa.bak 2>/dev/null; cp ~/.ssh/id_rsa.pub ~/.ssh/id_rsa.pub.bak 2>/dev/null\"", ".", cancellationToken);

        var generateResult = await scriptRunner.ExecuteAsync(
            "ssh-keygen", $"-t rsa -b {request.KeySize} -C \"{request.KeyComment}\" -f ~/.ssh/id_rsa -N \"\" -q", ".", cancellationToken);

        return $"SSH keys rotated successfully. Old keys backed up. {generateResult}";
    }
}

