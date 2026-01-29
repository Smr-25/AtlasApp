using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Decisions.Commands.CreateDecision;

public record CreateDecisionCommand(
    string Title,
    string? Description,
    DecisionPriority? Priority,
    Guid GoalId
) : IRequest<ResponseModel<DecisionDto>>;