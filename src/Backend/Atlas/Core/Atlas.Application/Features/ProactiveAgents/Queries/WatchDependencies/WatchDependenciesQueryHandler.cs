using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Queries.WatchDependencies;

public class WatchDependenciesQueryHandler(
    IProactiveAgentService agentService
) : IRequestHandler<WatchDependenciesQuery, List<string>>
{
    public async Task<List<string>> Handle(WatchDependenciesQuery request, CancellationToken cancellationToken)
    {
        return await agentService.AnalyzeDependenciesAsync(request.ProjectFilePath, cancellationToken);
    }
}

