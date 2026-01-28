using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Personas.Dtos;
using AutoMapper;
using MediatR;

namespace Atlas.Application.Features.Personas.Queries.GetPersonaById;

public class GetPersonaByIdQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetPersonaByIdQuery, ResponseModel<PersonaDto>>
{
    public async Task<ResponseModel<PersonaDto>> Handle(GetPersonaByIdQuery request,
        CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FindAsync(request.PersonaId, cancellationToken);
        if (persona is null)
            throw new NotFoundException("Persona not found");
        var personaDto = mapper.Map<PersonaDto>(persona);
        return ResponseModel<PersonaDto>.Success(personaDto);
    }
}