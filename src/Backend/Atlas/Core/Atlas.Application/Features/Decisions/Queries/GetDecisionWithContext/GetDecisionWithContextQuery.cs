using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Decisions.Queires.GetDecisionWithContext;

public record GetDecisionWithContextQuery(Guid DecisionId) : IRequest<ResponseModel<DecisionDetailDto>>;

public class GetDecisionWithContextQueryHandler(IApplicationDbContext applicationDbContext,IMapper mapper) : IRequestHandler<GetDecisionWithContextQuery, ResponseModel<DecisionDetailDto>>
{
    public async Task<ResponseModel<DecisionDetailDto>> Handle(GetDecisionWithContextQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}