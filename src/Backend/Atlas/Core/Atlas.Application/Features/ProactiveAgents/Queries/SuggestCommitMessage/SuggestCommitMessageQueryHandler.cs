using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Queries.SuggestCommitMessage;

public class SuggestCommitMessageQueryHandler(
    IProactiveAgentService agentService
) : IRequestHandler<SuggestCommitMessageQuery, string>
{
    public async Task<string> Handle(SuggestCommitMessageQuery request, CancellationToken cancellationToken)
    {
        return await agentService.SuggestCommitMessageAsync(request.DiffContent, cancellationToken);
    }
}

