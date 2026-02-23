using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Queries.SuggestAutoPatches;

public class SuggestAutoPatchesQueryHandler(
    ISecOpsAgentService agentService
) : IRequestHandler<SuggestAutoPatchesQuery, List<PatchSuggestion>>
{
    public async Task<List<PatchSuggestion>> Handle(SuggestAutoPatchesQuery request, CancellationToken cancellationToken)
    {
        return await agentService.SuggestAutoPatchesAsync(request.ProjectPath, cancellationToken);
    }
}

