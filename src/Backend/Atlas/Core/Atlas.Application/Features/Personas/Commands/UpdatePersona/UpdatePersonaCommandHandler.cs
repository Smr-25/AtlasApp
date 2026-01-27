using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Personas.Dtos;
using AutoMapper;
using MediatR;
namespace Atlas.Application.Features.Personas.Commands.UpdatePersona;

public class UpdatePersonaCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    : IRequestHandler<UpdatePersonaCommand, ResponseModel<PersonaDto>>
{
    public async Task<ResponseModel<PersonaDto>> Handle(UpdatePersonaCommand request,
        CancellationToken cancellationToken)
    {
        var persona = applicationDbContext.Personas.Find(request.Name);
        if (persona == null)
            throw new NotFoundException("Persona not found.");

        var existingPersonaWithName = applicationDbContext.Personas
            .FirstOrDefault(p => p.Name == request.Name && p.Name != request.Name);

        if (existingPersonaWithName != null)
            throw new AlreadyExistException("Another persona with the same name already exists.");

        persona.UpdateName(request.Name);
        
        var existingPersonaWithAlias = applicationDbContext.Personas
            .FirstOrDefault(p => p.Alias == request.Alias && p.Name != request.Name);

        if (existingPersonaWithAlias != null)
            throw new AlreadyExistException("Another persona with the same alias already exists.");

        persona.UpdateAlias(request.Alias);

        applicationDbContext.Personas.Update(persona);
        await applicationDbContext.SaveChangesAsync();

        var personaDto = mapper.Map<PersonaDto>(persona);
        return ResponseModel<PersonaDto>.Success(personaDto);
    }
}