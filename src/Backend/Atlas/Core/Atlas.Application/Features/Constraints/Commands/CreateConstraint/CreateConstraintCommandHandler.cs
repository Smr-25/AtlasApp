using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Constraints.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Constraints.Commands.CreateConstraint;

public class CreateConstraintCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateConstraintCommand, ResponseModel<ConstraintDto>>
{
    public async Task<ResponseModel<ConstraintDto>> Handle(CreateConstraintCommand request,
        CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FirstOrDefaultAsync(x => x.UserId.Equals(currentUserService.UserId), cancellationToken);

        if (persona == null)
            throw new NotFoundException("Persona not found for the current user.");

        var constraint = Constraint.Create(
            persona.Id,
            request.Type,
            request.Description,
            request.ImpactLevel,
            null);

        await applicationDbContext.Constraints.AddAsync(constraint, cancellationToken);
        await applicationDbContext.SaveChangesAsync();
        var constraintDto = mapper.Map<ConstraintDto>(constraint);
        return ResponseModel<ConstraintDto>.Success(constraintDto);
    }
}