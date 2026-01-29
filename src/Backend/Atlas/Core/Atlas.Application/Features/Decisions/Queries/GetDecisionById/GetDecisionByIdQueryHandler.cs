using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Decisions.Queries.GetDecisionById;

public class GetDecisionByIdQueryHandler(IApplicationDbContext applicationDbContext,IMapper mapper) : IRequestHandler<GetDecisionByIdQuery, ResponseModel<DecisionDto>>
{
    public async Task<ResponseModel<DecisionDto>> Handle(GetDecisionByIdQuery request, CancellationToken cancellationToken)
    {
        var decision = await applicationDbContext.Decisions.FirstOrDefaultAsync(d=>d.Id == request.DecisionId, cancellationToken);
        if(decision == null)
            throw new NotFoundException("Decision not found");
        var decisionDto = mapper.Map<DecisionDto>(decision);
        return ResponseModel<DecisionDto>.Success(decisionDto);
    }
}