using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Personas.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.CreatePersona;

public class CreatePersonaCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper)
    : IRequestHandler<CreatePersonaCommand, ResponseModel<PersonaDto>>
{
    public async Task<ResponseModel<PersonaDto>> Handle(CreatePersonaCommand request,
        CancellationToken cancellationToken)
    {
        var persona = Persona.Create(
            userId: new Guid(currentUserService.UserId!),
            name: request.Name,
            alias: request.Alias
        );

        applicationDbContext.Personas.Add(persona);
        await applicationDbContext.SaveChangesAsync();
        var personaDto = mapper.Map<PersonaDto>(persona);
        return ResponseModel<PersonaDto>.Success(personaDto);
    }
}