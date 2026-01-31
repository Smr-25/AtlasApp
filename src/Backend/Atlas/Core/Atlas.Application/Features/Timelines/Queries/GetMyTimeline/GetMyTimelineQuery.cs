using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Timelines.Queries.GetMyTimeline;

public record GetMyTimelineQuery(int? PageNumber, int? PageSize) : IRequest<ResponseModel<PagedResult>>;