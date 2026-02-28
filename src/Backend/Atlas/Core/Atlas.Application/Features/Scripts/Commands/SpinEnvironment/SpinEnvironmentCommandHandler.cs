using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Path = System.IO.Path;

namespace Atlas.Application.Features.Scripts.Commands.SpinEnvironment;

public class SpinEnvironmentCommandHandler(
    IScriptRunnerService scriptRunner,
    IAtlasHubService hubService,
    ICurrentUserService currentUser
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

        var userId = currentUser.GetUserIdOrDefault();
        if (userId != null)
        {
            await hubService.SendJobCompletedAsync(userId.Value, "SpinEnvironment", new
            {
                Output = result
            }, cancellationToken);
        }

        return result;
    }
}
