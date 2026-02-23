using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Commands.ResendLowOpenRate;

public class ResendLowOpenRateCommandHandler(
    IMarketerAgentService agentService
) : IRequestHandler<ResendLowOpenRateCommand, string>
{
    public async Task<string> Handle(ResendLowOpenRateCommand request, CancellationToken cancellationToken)
    {
        return await agentService.ResendLowOpenRateAsync(request.CampaignId, request.NewSubject, cancellationToken);
    }
}

