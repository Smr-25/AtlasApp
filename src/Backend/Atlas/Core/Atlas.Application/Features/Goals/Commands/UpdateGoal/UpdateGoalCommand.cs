using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Dtos;
using MediatR;

namespace Atlas.Application.Features.Goals.Commands.UpdateGoal;

public record UpdateGoalCommand(
    Guid GoalId,
    string? Title,
    string? Description,
    int? Priority,
    DateTime? DueDate
) : IRequest<ResponseModel<GoalDto>>;