using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Timelines.Queries.GetTimelineByDateRange;

public record GetTimelineByDateRangeQuery(
    DateTime DateFrom,
    DateTime DateTo,
    int? PageNumber,
    int? PageSize
) : IRequest<ResponseModel<PagedResult>>;