using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using AutoMapper;
using MediatR;

namespace Atlas.Application.Features.Decisions.Queires.GetDecisionWithContext;

public class GetDecisionWithContextQueryHandler(IApplicationDbContext applicationDbContext,IMapper mapper) : IRequestHandler<GetDecisionWithContextQuery, ResponseModel<DecisionDetailDto>>
{
    public async Task<ResponseModel<DecisionDetailDto>> Handle(GetDecisionWithContextQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}