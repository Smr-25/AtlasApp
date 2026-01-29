using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using MediatR;

namespace Atlas.Application.Features.Decisions.Commands.ExecuteDecision;

public record ExecuteDecisionCommand(
    Guid DecisionId
) : IRequest<ResponseModel<DecisionDto>>;