using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using MediatR;

namespace Atlas.Application.Features.Decisions.Commands.PostponeDecision;

public record PostponeDecisionCommand(
    Guid DecisionId,
    string? Note
) : IRequest<ResponseModel<DecisionDto>>;