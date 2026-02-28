using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunQuickScan;

public class RunQuickScanCommandHandler(
    IScriptRunnerService scriptRunner,
    IAtlasHubService hubService,
    ICurrentUserService currentUser
) : IRequestHandler<RunQuickScanCommand, string>
{
    public async Task<string> Handle(RunQuickScanCommand request, CancellationToken cancellationToken)
    {
        var result = await scriptRunner.ExecuteAsync(
            "arp", $"-a", ".", cancellationToken);

        var userId = currentUser.GetUserIdOrDefault();
        if (userId != null)
        {
            await hubService.SendJobCompletedAsync(userId.Value, "QuickScan", new
            {
                Output = result
            }, cancellationToken);
        }

        return result;
    }
}
