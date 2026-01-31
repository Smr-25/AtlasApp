using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Commands.CompleteGoal;
using Atlas.Application.Features.Goals.Dtos;
using MediatR;

namespace Atlas.Application.Features.Goals.Commands.AbandonGoal;

public record AbandonGoalCommand(Guid GoalId) : IRequest<ResponseModel<GoalDto>>;