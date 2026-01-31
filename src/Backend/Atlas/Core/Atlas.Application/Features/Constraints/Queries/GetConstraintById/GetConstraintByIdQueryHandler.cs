using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Constraints.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Constraints.Queries.GetConstraintById;

public class GetConstraintByIdQueryHandler(
    IApplicationDbContext applicationDbContext,
    IMapper mapper) : IRequestHandler<GetConstraintByIdQuery, ResponseModel<ConstraintDto>>
{
    public async Task<ResponseModel<ConstraintDto>> Handle(GetConstraintByIdQuery request,
        CancellationToken cancellationToken)
    {
        var constraint = await applicationDbContext.Constraints
            .FirstOrDefaultAsync(c => c.Id == request.ConstraintId, cancellationToken);

        if (constraint == null)
            throw new NotFoundException("Constraint not found.");

        var constraintDto = mapper.Map<ConstraintDto>(constraint);
        return ResponseModel<ConstraintDto>.Success(constraintDto);
    }
}