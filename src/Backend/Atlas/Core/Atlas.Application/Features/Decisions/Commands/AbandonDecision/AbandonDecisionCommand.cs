using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Commands.ExecuteDecision;
using Atlas.Application.Features.Decisions.Dtos;
using MediatR;

namespace Atlas.Application.Features.Decisions.Commands.AbandonDecision;

public record AbandonDecisionCommand(
    Guid DecisionId,
    string? Reason
) : IRequest<ResponseModel<DecisionDto>>;