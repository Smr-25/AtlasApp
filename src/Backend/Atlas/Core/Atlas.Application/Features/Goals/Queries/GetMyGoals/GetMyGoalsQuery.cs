using Atlas.Application.Common.Models;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Goals.Queries.GetMyGoals;

public record GetMyGoalsQuery(
    int? PageNumber,
    int? PageSize,
    GoalStatus? Status
) : IRequest<ResponseModel<PagedResult>>;