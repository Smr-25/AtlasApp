using Atlas.Application.Common.Models;
using Atlas.Application.Features.Constraints.Dtos;
using MediatR;

namespace Atlas.Application.Features.Constraints.Queries.GetActiveConstraints;

public record GetActiveConstraintsQuery : IRequest<ResponseModel<List<ConstraintDto>>>;
