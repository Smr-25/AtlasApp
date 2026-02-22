using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.FormatAndLint;

public class FormatAndLintCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<FormatAndLintCommand, string>
{
    public async Task<string> Handle(FormatAndLintCommand request, CancellationToken cancellationToken)
    {
        var formatResult = await scriptRunner.ExecuteAsync(
            "dotnet", "format", request.ProjectPath, cancellationToken);

        return $"Format: {formatResult}";
    }
}

