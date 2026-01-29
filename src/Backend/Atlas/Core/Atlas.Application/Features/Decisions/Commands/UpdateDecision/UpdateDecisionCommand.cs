using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Decisions.Commands.UpdateDecision;

public record UpdateDecisionCommand(
    Guid DecisionId,
    string? Title,
    string? Description,
    DecisionPriority? Priority
) : IRequest<ResponseModel<DecisionDto>>;