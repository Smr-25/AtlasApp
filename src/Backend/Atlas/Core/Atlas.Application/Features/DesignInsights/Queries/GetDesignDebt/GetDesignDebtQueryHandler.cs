using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DesignInsights.Queries.GetDesignDebt;

public class GetDesignDebtQueryHandler(
    IDesignInsightCalculationService designInsight,
    ICurrentUserService currentUser
) : IRequestHandler<GetDesignDebtQuery, int>
{
    public async Task<int> Handle(GetDesignDebtQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await designInsight.GetDesignDebtCountAsync(userId, cancellationToken);
    }
}

