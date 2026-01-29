using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using MediatR;

namespace Atlas.Application.Features.Decisions.Queires.GetDecisionWithContext;

public record GetDecisionWithContextQuery(Guid DecisionId) : IRequest<ResponseModel<DecisionDetailDto>>;