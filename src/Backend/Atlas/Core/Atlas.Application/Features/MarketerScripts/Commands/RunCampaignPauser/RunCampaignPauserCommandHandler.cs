using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunCampaignPauser;

public class RunCampaignPauserCommandHandler(
    IMetaAdsAdapter metaAds
) : IRequestHandler<RunCampaignPauserCommand, string>
{
    public async Task<string> Handle(RunCampaignPauserCommand request, CancellationToken cancellationToken)
    {
        var results = new List<string>();
        foreach (var id in request.CampaignIds)
        {
            var result = await metaAds.PauseCampaignAsync(id, cancellationToken);
            results.Add($"Campaign {id}: {result}");
        }
        return string.Join("\n", results);
    }
}

