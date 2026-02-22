using MediatR;

namespace Atlas.Application.Features.DevInsights.Queries.GetTimeSaved;

public record GetTimeSavedQuery(DateTime From, DateTime To) : IRequest<TimeSavedResult>;

public record TimeSavedResult(double TotalHoursSaved, int ScriptsRun, int AutomationsTriggered);

