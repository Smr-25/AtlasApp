using Atlas.Application.Common.Models;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Decisions.Queries.GetDecisionsByStatus;

public record GetDecisionsByStatusQuery(
    DecisionStatus Status,
    int? PageNumber,
    int PageSize
) : IRequest<ResponseModel<PagedResult>>;