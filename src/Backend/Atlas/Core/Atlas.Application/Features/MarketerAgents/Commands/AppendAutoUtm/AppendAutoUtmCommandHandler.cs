using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Commands.AppendAutoUtm;

public class AppendAutoUtmCommandHandler(
    IMarketerAgentService agentService
) : IRequestHandler<AppendAutoUtmCommand, string>
{
    public async Task<string> Handle(AppendAutoUtmCommand request, CancellationToken cancellationToken)
    {
        return await agentService.AppendUtmAsync(request.Url, request.Source, request.Medium, request.Campaign, cancellationToken);
    }
}

