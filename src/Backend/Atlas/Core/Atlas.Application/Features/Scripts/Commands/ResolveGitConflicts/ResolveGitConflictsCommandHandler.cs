using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.ResolveGitConflicts;

public class ResolveGitConflictsCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<ResolveGitConflictsCommand, string>
{
    public async Task<string> Handle(ResolveGitConflictsCommand request, CancellationToken cancellationToken)
    {
        var commands = $"git fetch origin && git stash && git pull --rebase -X {request.Strategy} origin && git stash pop";
        return await scriptRunner.ExecuteAsync("bash", $"-c \"{commands}\"", request.RepositoryPath, cancellationToken);
    }
}

