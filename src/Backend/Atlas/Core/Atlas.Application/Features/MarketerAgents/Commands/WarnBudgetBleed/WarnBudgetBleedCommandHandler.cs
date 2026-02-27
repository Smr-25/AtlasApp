using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.MarketerAgents.Commands.WarnBudgetBleed;

public class WarnBudgetBleedCommandHandler(
    IMarketerAgentService agentService,
    ICurrentUserService currentUser,
    IApplicationDbContext dbContext,
    IAtlasHubService hubService
) : IRequestHandler<WarnBudgetBleedCommand, BudgetBleedResult>
{
    public async Task<BudgetBleedResult> Handle(WarnBudgetBleedCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var result = await agentService.DetectBudgetBleedAsync(userId, cancellationToken);

        if (result.HasBleed)
        {
            var member = await dbContext.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == userId, cancellationToken);
            if (member != null)
            {
                await hubService.SendAlertAsync(member.TeamId, "BudgetBleedDetected", new
                {
                    DetectedBy = currentUser.UserName,
                    BleedingCampaigns = result.Campaigns,
                    Severity = "High"
                }, cancellationToken);
            }
        }

        return result;
    }
}

