using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Dtos;
using MediatR;

namespace Atlas.Application.Features.Goals.Commands.PauseGoal;

public record PauseGoalCommand(Guid GoalId): IRequest<ResponseModel<GoalDto>>;