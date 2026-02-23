using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunWeeklyReport;

public class RunWeeklyReportCommandHandler(
    IMarketerInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<RunWeeklyReportCommand, string>
{
    public async Task<string> Handle(RunWeeklyReportCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var roas = await insightService.GetTotalRoasAsync(userId, request.From, request.To, cancellationToken);
        var leads = await insightService.GetLeadsGeneratedAsync(userId, request.From, request.To, cancellationToken);
        return $"Weekly Report ({request.From:d} - {request.To:d})\nROAS: {roas:F2}x\nLeads: {leads}";
    }
}

