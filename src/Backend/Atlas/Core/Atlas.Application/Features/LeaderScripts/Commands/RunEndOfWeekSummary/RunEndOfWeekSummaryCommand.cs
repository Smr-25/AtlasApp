using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunEndOfWeekSummary;

public record RunEndOfWeekSummaryCommand(Guid TeamId) : IRequest<WeekSummaryResult>;

