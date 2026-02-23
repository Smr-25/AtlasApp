using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunLocalWipe;

public class RunLocalWipeCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<RunLocalWipeCommand, string>
{
    public async Task<string> Handle(RunLocalWipeCommand request, CancellationToken cancellationToken)
    {
        var results = new List<string>();

        if (request.WipeHistory)
        {
            var historyResult = await scriptRunner.ExecuteAsync(
                "bash", "-c \"rm -f ~/.bash_history ~/.zsh_history && history -c\"", ".", cancellationToken);
            results.Add($"History wiped: {historyResult}");
        }

        if (request.WipeCredentials)
        {
            var credResult = await scriptRunner.ExecuteAsync(
                "bash", "-c \"rm -rf ~/.ssh/known_hosts && security delete-generic-password -a '' 2>/dev/null\"", ".", cancellationToken);
            results.Add($"Credentials wiped: {credResult}");
        }

        return string.Join("\n", results);
    }
}

