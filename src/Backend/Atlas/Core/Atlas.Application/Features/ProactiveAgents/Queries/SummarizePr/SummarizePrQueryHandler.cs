using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Queries.SummarizePr;

public class SummarizePrQueryHandler(
    IProactiveAgentService agentService
) : IRequestHandler<SummarizePrQuery, string>
{
    public async Task<string> Handle(SummarizePrQuery request, CancellationToken cancellationToken)
    {
        return await agentService.SummarizePrAsync(request.PrDiff, request.PrTitle, cancellationToken);
    }
}

