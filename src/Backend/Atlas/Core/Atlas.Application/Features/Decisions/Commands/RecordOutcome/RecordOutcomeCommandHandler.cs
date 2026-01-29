using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Decisions.Commands.RecordOutcome;

public class RecordOutcomeCommandHandler(IApplicationDbContext applicationDbContext,IMapper mapper) : IRequestHandler<RecordOutcomeCommand, ResponseModel<DecisionOutcomeDto>>
{
    public async Task<ResponseModel<DecisionOutcomeDto>> Handle(RecordOutcomeCommand request, CancellationToken cancellationToken)
    {
        var decision = await applicationDbContext.Decisions.FirstOrDefaultAsync(d=>d.Id == request.DecisionId, cancellationToken);

        if (decision == null)
            throw new NotFoundException($"Decision with id {request.DecisionId} does not exist.");

        var outcome = DecisionOutcome.Record(
            request.DecisionId,
            request.Status,
            request.Description,
            request.WasExpected,
            request.LessonLearned
        );
        applicationDbContext.DecisionOutcomes.Add(outcome);
        await applicationDbContext.SaveChangesAsync();
        var outcomeDto = mapper.Map<DecisionOutcomeDto>(outcome);
        return ResponseModel<DecisionOutcomeDto>.Success(outcomeDto);
    }
}