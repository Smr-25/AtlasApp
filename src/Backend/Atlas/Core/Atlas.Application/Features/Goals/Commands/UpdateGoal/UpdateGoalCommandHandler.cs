using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Goals.Commands.UpdateGoal;

public class UpdateGoalCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    : IRequestHandler<UpdateGoalCommand, ResponseModel<GoalDto>>
{
    public async Task<ResponseModel<GoalDto>> Handle(UpdateGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await applicationDbContext.Goals.FirstOrDefaultAsync(g => g.Id == request.GoalId, cancellationToken);
        if (goal == null)
            throw new NotFoundException("Goal not found.");
        if (request.Title != null)
            goal.GetType().GetProperty("Title")!.SetValue(goal, request.Title);
        if (request.Description != null)
            goal.GetType().GetProperty("Description")!.SetValue(goal, request.Description);
        if (request.Priority != null)
            goal.GetType().GetProperty("Priority")!.SetValue(goal, request.Priority);
        if (request.DueDate != null)
            goal.GetType().GetProperty("DueDate")!.SetValue(goal, request.DueDate);
        await applicationDbContext.SaveChangesAsync();
        var goalDto = mapper.Map<GoalDto>(goal);
        return ResponseModel<GoalDto>.Success(goalDto);
    }
}