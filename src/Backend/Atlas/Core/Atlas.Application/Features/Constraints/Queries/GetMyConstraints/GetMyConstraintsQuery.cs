using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Constraints.Queries.GetMyConstraints;

public record GetMyConstraintsQuery(
    int? PageNumber,
    int? PageSize,
    bool? IsActive
) : IRequest<ResponseModel<PagedResult>>;