using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.System.Queries.GetAiAnalysis;

public class GetAiAnalysisQueryHandler(ISystemMonitorService monitorService, IAiAdvisorService aiService)
    : IRequestHandler<GetAiAnalysisQuery, SystemAnalysisResult>
{
    public async Task<SystemAnalysisResult> Handle(GetAiAnalysisQuery request, CancellationToken cancellationToken)
    {
        var snapshot = await monitorService.GetSnapshotAsync();
        var advice = await aiService.AnalyzeHealthAsync(snapshot);

        return new SystemAnalysisResult(snapshot, advice);
    }
}