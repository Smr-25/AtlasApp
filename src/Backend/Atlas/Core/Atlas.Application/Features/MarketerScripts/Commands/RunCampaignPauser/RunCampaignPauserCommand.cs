using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunCampaignPauser;

public record RunCampaignPauserCommand(List<string> CampaignIds, string Reason) : IRequest<string>;

