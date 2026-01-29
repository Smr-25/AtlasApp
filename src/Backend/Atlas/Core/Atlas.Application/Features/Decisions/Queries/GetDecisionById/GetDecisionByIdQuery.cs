using Atlas.Application.Common.Models;
using Atlas.Application.Features.Decisions.Dtos;
using MediatR;

namespace Atlas.Application.Features.Decisions.Queries.GetDecisionById;

public record GetDecisionByIdQuery(Guid DecisionId) : IRequest<ResponseModel<DecisionDto>>;