using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Personas.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Queries.GetMyPersona;

public class GetMyPersonaQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetMyPersonaQuery, ResponseModel<PersonaDto>>
{
    public async Task<ResponseModel<PersonaDto>> Handle(GetMyPersonaQuery request, CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FirstOrDefaultAsync(p => p.UserId.Equals(currentUserService.UserId), cancellationToken);
        if (persona == null)
            throw new NotFoundException("Persona not found");

        var personaDto = mapper.Map<PersonaDto>(persona);
        return ResponseModel<PersonaDto>.Success(personaDto);
    }
}