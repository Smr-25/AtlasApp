using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Decisions.Commands.UpdateDecision;

public class UpdateDecisionCommandHandler(
    IApplicationDbContext applicationDbContext,
    IMapper mapper)
    : IRequestHandler<UpdateDecisionCommand, ResponseModel<DecisionDto>>
{
    public async Task<ResponseModel<DecisionDto>> Handle(UpdateDecisionCommand request,
        CancellationToken cancellationToken)
    {
        var decision = await applicationDbContext.Decisions.FirstOrDefaultAsync(
            d => d.Id == request.DecisionId,
            cancellationToken);
        if (decision is null)
            throw new NotFoundException($"Decision with id {request.DecisionId} not found.");
        var updatedDecision = decision.Update(
            request.Title,
            request.Description,
            request.Priority
        );
        applicationDbContext.Decisions.Update(updatedDecision);
        await applicationDbContext.SaveChangesAsync();
        var decisionDto = mapper.Map<DecisionDto>(updatedDecision);
        return ResponseModel<DecisionDto>.Success(decisionDto);
    }
}