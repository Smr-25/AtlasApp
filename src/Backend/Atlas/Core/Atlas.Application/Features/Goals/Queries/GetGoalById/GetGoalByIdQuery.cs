using Atlas.Application.Common.Models;
using Atlas.Application.Features.Goals.Dtos;
using MediatR;

namespace Atlas.Application.Features.Goals.Queries.GetGoalById;

public record GetGoalByIdQuery(Guid GoalId): IRequest<ResponseModel<GoalDto>>;