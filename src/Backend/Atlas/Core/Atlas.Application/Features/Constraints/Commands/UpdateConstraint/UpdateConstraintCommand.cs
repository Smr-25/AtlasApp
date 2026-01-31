using Atlas.Application.Common.Models;
using Atlas.Application.Features.Constraints.Dtos;
using MediatR;

namespace Atlas.Application.Features.Constraints.Commands.UpdateConstraint;

public record UpdateConstraintCommand(
    Guid ConstraintId,
    string? Description,
    int? ImpactLevel
) : IRequest<ResponseModel<ConstraintDto>>;