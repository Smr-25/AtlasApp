using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.PersonaStates.Queries.GetStateHistory;

public class GetStateHistoryQuery(
    int? PageNumber,
    int? PageSize,
    DateTime? DateFrom,
    DateTime? DateTo
) : IRequest<ResponseModel<PagedResult>>;