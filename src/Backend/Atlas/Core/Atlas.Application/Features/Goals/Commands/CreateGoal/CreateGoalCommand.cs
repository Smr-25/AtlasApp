using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Dtos;
using MediatR;

namespace Atlas.Application.Features.Goals.Commands.CreateGoal;

public record CreateGoalCommand(
    string Title,
    string? Description,
    int Priority,
    DateTime? DueDate
) : IRequest<ResponseModel<GoalDto>>;