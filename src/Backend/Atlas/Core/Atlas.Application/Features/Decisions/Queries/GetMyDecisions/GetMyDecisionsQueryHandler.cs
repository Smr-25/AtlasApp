using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using AutoMapper;
using MediatR;

namespace Atlas.Application.Features.Decisions.Queries.GetMyDecisions;

public class GetMyDecisionsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IMapper mapper)
    : IRequestHandler<GetMyDecisionsQuery, ResponseModel<PagedResult>>
{
    public Task<ResponseModel<PagedResult>> Handle(GetMyDecisionsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}