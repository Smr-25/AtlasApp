using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Dtos;
using MediatR;

namespace Atlas.Application.Features.Goals.Commands.UpdateGoalProgress;

public record UpdateGoalProgressCommand(
    Guid GoalId,
    int ProgressPercentage
) : IRequest<ResponseModel<GoalDto>>;