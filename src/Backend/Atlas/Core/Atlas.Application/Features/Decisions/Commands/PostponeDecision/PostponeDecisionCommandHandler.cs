using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Decisions.Commands.PostponeDecision;

public class PostponeDecisionCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<PostponeDecisionCommand, ResponseModel<DecisionDto>>
{
    public async Task<ResponseModel<DecisionDto>> Handle(PostponeDecisionCommand request,
        CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FirstOrDefaultAsync(x => x.UserId.ToString() == currentUserService.UserId, cancellationToken);

        if (persona is null)
            throw new NotFoundException("Persona for current user not found");

        var decision = await applicationDbContext.Decisions
            .FirstOrDefaultAsync(x => x.Id == request.DecisionId && x.PersonaId == persona.Id, cancellationToken);

        if (decision is null)
            throw new NotFoundException("Decision not found");

        decision.Postpone(request.Note);
        await applicationDbContext.SaveChangesAsync();

        var decisionDto = mapper.Map<DecisionDto>(decision);
        return ResponseModel<DecisionDto>.Success(decisionDto);
    }
}