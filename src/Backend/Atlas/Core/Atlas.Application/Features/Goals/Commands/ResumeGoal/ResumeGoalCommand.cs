using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Dtos;
using MediatR;

namespace Atlas.Application.Features.Goals.Commands.ResumeGoal;

public record ResumeGoalCommand(Guid GoalId) : IRequest<ResponseModel<GoalDto>>;