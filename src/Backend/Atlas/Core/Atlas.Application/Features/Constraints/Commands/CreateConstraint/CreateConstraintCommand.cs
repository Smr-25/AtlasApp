using Atlas.Application.Common.Models;
using Atlas.Application.Features.Constraints.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Constraints.Commands.CreateConstraint;

public record CreateConstraintCommand(
    ConstraintType Type,
    string Description,
    int ImpactLevel
) : IRequest<ResponseModel<ConstraintDto>>;