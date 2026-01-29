using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Decisions.Commands.ExecuteDecision;

public class ExecuteDecisionCommandHandler(
    IApplicationDbContext applicationDbContext,
    IMapper mapper) : IRequestHandler<ExecuteDecisionCommand, ResponseModel<DecisionDto>>
{
    public async Task<ResponseModel<DecisionDto>> Handle(ExecuteDecisionCommand request,
        CancellationToken cancellationToken)
    {
        var decision = await applicationDbContext.Decisions
            .FirstOrDefaultAsync(x => x.Id == request.DecisionId, cancellationToken);

        if (decision is null)
            throw new NotFoundException("Decision not found");

        decision.Execute();
        await applicationDbContext.SaveChangesAsync();
        var decisionDto = mapper.Map<DecisionDto>(decision);
        return ResponseModel<DecisionDto>.Success(decisionDto);
    }
}