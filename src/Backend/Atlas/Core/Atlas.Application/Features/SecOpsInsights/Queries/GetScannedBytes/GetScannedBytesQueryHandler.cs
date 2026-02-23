using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetScannedBytes;

public class GetScannedBytesQueryHandler(
    ISecOpsInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetScannedBytesQuery, ScannedBytesResult>
{
    public async Task<ScannedBytesResult> Handle(GetScannedBytesQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var bytes = await insightService.GetScannedBytesAsync(userId, request.From, request.To, cancellationToken);
        var formatted = bytes switch
        {
            >= 1_073_741_824 => $"{bytes / 1_073_741_824.0:F2} GB",
            >= 1_048_576 => $"{bytes / 1_048_576.0:F2} MB",
            >= 1_024 => $"{bytes / 1_024.0:F2} KB",
            _ => $"{bytes} B"
        };
        return new ScannedBytesResult(bytes, formatted);
    }
}

