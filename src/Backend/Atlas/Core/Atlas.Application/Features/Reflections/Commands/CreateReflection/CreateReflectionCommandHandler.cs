using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Reflections.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Reflections.Commands.CreateReflection;

public class CreateReflectionCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateReflectionCommand, ResponseModel<ReflectionDto>>
{
    public async Task<ResponseModel<ReflectionDto>> Handle(CreateReflectionCommand request,
        CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FirstOrDefaultAsync(p => p.Id.Equals(currentUserService.UserId), cancellationToken);
        if (persona is null)
            throw new NotFoundException("Persona not found for the current user.");

        var reflection = Reflection.Create(
            persona.Id,
            request.Content,
            request.ReflectionType,
            request.DecisionId != Guid.Empty ? request.DecisionId : null,
            request.MoodScore,
            request.Tags.Where(tag => tag is not null).Select(tag => tag!).ToList()
        );
        await applicationDbContext.Reflections.AddAsync(reflection, cancellationToken);
        await applicationDbContext.SaveChangesAsync();
        var reflectionDto = mapper.Map<ReflectionDto>(reflection);
        return ResponseModel<ReflectionDto>.Success(reflectionDto);
    }
}