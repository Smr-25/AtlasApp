using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Constraints.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Constraints.Commands.UpdateConstraint;

public class UpdateConstraintCommandHandler(
    IApplicationDbContext applicationDbContext,
    IMapper mapper) : IRequestHandler<UpdateConstraintCommand, ResponseModel<ConstraintDto>>
{
    public async Task<ResponseModel<ConstraintDto>> Handle(UpdateConstraintCommand request,
        CancellationToken cancellationToken)
    {
        var constraint = await applicationDbContext.Constraints
            .FirstOrDefaultAsync(c => c.Id == request.ConstraintId, cancellationToken);

        if (constraint == null)
            throw new NotFoundException("Constraint not found.");

        if (request.ImpactLevel is not null)
            constraint.UpdateImpactLevel(request.ImpactLevel.Value);

        await applicationDbContext.SaveChangesAsync();

        var constraintDto = mapper.Map<ConstraintDto>(constraint);
        return ResponseModel<ConstraintDto>.Success(constraintDto);
    }
}