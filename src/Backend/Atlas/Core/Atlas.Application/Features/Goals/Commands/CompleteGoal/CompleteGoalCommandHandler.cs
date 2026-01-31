using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Goals.Commands.CompleteGoal;

public class CompleteGoalCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    : IRequestHandler<CompleteGoalCommand, ResponseModel<GoalDto>>
{
    public async Task<ResponseModel<GoalDto>> Handle(CompleteGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await applicationDbContext.Goals.FirstOrDefaultAsync(g => g.Id == request.GoalId, cancellationToken);
        if (goal == null)
            throw new NotFoundException("Goal not found.");
        goal.Complete();
        await applicationDbContext.SaveChangesAsync();
        var goalDto = mapper.Map<GoalDto>(goal);
        return ResponseModel<GoalDto>.Success(goalDto);
    }
}