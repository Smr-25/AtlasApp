using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Commands.ResendLowOpenRate;

public record ResendLowOpenRateCommand(string CampaignId, string NewSubject) : IRequest<string>;

