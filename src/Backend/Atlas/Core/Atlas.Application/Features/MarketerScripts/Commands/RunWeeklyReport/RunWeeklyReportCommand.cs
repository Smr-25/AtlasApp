using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunWeeklyReport;

public record RunWeeklyReportCommand(DateTime From, DateTime To) : IRequest<string>;

