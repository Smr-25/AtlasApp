using MediatR;

namespace Atlas.Application.Features.DevUtilities.Queries.GenerateCron;

public record GenerateCronQuery(string Description) : IRequest<GenerateCronResult>;

public record GenerateCronResult(string CronExpression, string HumanReadable);

