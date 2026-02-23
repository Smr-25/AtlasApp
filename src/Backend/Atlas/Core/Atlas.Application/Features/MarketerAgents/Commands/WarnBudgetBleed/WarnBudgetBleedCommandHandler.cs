using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Commands.WarnBudgetBleed;

public class WarnBudgetBleedCommandHandler(
    IMarketerAgentService agentService,
    ICurrentUserService currentUser
) : IRequestHandler<WarnBudgetBleedCommand, BudgetBleedResult>
{
    public async Task<BudgetBleedResult> Handle(WarnBudgetBleedCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await agentService.DetectBudgetBleedAsync(userId, cancellationToken);
    }
}

