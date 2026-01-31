using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Dtos;
using MediatR;

namespace Atlas.Application.Features.Goals.Commands.CompleteGoal;

public record CompleteGoalCommand(Guid GoalId) : IRequest<ResponseModel<GoalDto>>;