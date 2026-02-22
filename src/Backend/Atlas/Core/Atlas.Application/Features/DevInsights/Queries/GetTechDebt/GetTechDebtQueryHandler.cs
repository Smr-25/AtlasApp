using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevInsights.Queries.GetTechDebt;

public class GetTechDebtQueryHandler(
    IInsightCalculationService insightService
) : IRequestHandler<GetTechDebtQuery, TechDebtResult>
{
    public async Task<TechDebtResult> Handle(GetTechDebtQuery request, CancellationToken cancellationToken)
    {
        var count = await insightService.CountTodoCommentsAsync(request.ProjectPath, cancellationToken);
        return new TechDebtResult(count, 0, 0, count);
    }
}

