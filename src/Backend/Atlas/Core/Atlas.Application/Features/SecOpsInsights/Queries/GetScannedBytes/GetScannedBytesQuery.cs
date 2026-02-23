using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetScannedBytes;

public record GetScannedBytesQuery(DateTime From, DateTime To) : IRequest<ScannedBytesResult>;

public record ScannedBytesResult(long TotalBytes, string FormattedSize);

