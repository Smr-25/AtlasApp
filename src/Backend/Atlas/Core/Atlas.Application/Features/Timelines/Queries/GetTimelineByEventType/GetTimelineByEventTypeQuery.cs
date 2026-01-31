using Atlas.Application.Common.Models;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Timelines.Queries.GetTimelineByEventType;

public record GetTimelineByEventTypeQuery(
    TimelineEventType EventType,
    int? PageNumber,
    int? PageSize
) : IRequest<ResponseModel<PagedResult>>;
