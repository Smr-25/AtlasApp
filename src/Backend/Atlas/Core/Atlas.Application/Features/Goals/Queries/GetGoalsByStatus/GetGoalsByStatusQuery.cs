using Atlas.Application.Common.Models;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Goals.Queries.GetGoalsByStatus;

public record GetGoalsByStatusQuery(
    GoalStatus Status,
    int? PageNumber,
    int? PageSize
) : IRequest<ResponseModel<PagedResult>>;