using Atlas.Application.Common.Interfaces;
using MediatR;
using Path = System.IO.Path;

namespace Atlas.Application.Features.Scripts.Commands.SpinEnvironment;

public class SpinEnvironmentCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<SpinEnvironmentCommand, string>
{
    public async Task<string> Handle(SpinEnvironmentCommand request, CancellationToken cancellationToken)
    {
        var workDir = request.ProjectPath ?? Path.GetDirectoryName(request.DockerComposePath) ?? ".";
        var result = await scriptRunner.ExecuteAsync(
            "docker-compose",
            $"-f \"{request.DockerComposePath}\" up -d",
            workDir,
            cancellationToken);
        return result;
    }
}

