using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Goals.Commands.CreateGoal;

public class CreateGoalCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateGoalCommand, ResponseModel<GoalDto>>
{
    public async Task<ResponseModel<GoalDto>> Handle(CreateGoalCommand request, CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas.FirstOrDefaultAsync(
            p => p.UserId.Equals(currentUserService.UserId), cancellationToken);
        if (persona == null)
            throw new NotFoundException("Persona not found for the current user.");
        var goal = Goal.Create(
            persona.Id,
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate
        );
        await applicationDbContext.Goals.AddAsync(goal, cancellationToken);
        await applicationDbContext.SaveChangesAsync();
        var goalDto = mapper.Map<GoalDto>(goal);
        return ResponseModel<GoalDto>.Success(goalDto);
    }
}