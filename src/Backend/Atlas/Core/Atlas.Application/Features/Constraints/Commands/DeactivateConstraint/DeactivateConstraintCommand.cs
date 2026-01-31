using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Constraints.Commands.DeactivateConstraint;

public record DeactivateConstraintCommand(Guid ConstraintId) : IRequest<ResponseModel<bool>>;