using Atlas.Application.Common.Models;
using Atlas.Application.Features.Constraints.Dtos;
using MediatR;

namespace Atlas.Application.Features.Constraints.Queries.GetConstraintById;

public record GetConstraintByIdQuery(Guid ConstraintId) : IRequest<ResponseModel<ConstraintDto>>;

