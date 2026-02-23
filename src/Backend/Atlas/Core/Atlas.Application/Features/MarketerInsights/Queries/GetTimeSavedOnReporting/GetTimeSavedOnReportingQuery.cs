using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetTimeSavedOnReporting;

public record GetTimeSavedOnReportingQuery(DateTime From, DateTime To) : IRequest<TimeSavedOnReportingResult>;

public record TimeSavedOnReportingResult(double HoursSaved, int ReportsGenerated);

