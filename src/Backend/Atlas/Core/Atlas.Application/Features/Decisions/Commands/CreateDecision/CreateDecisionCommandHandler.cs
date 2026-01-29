using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Decisions.Commands.CreateDecision;

public class CreateDecisionCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateDecisionCommand, ResponseModel<DecisionDto>>
{
    public async Task<ResponseModel<DecisionDto>> Handle(CreateDecisionCommand request,
        CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FirstOrDefaultAsync(x => x.UserId.ToString() == currentUserService.UserId, cancellationToken);
        if (persona is null)
            throw new NotFoundException("Persona for current user not found");
        
        var decision = Decision.Create(
            personaId: persona.Id,
            title: request.Title,
            description: request.Description,
            priority: request.Priority!.Value,
            relatedGoalId: request.GoalId
        );

        await applicationDbContext.Decisions.AddAsync(decision, cancellationToken);
        await applicationDbContext.SaveChangesAsync();
        var decisionDto = mapper.Map<DecisionDto>(decision);
        return ResponseModel<DecisionDto>.Success(decisionDto);
    }
}