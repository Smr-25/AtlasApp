using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Constraints.Commands.DeactivateConstraint;

public class DeactivateConstraintCommandHanler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<DeactivateConstraintCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(DeactivateConstraintCommand request,
        CancellationToken cancellationToken)
    {
        var constraint = await applicationDbContext.Constraints
            .FirstOrDefaultAsync(c => c.Id == request.ConstraintId, cancellationToken);

        if (constraint == null)
            throw new NotFoundException("Constraint not found.");

        constraint.Deactivate();
        await applicationDbContext.SaveChangesAsync();

        return ResponseModel<bool>.Success(true);
    }
}