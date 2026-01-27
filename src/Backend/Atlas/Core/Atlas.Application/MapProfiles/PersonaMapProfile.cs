using Atlas.Application.Features.Personas.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;

namespace Atlas.Application.MapProfiles;

public class PersonaMapProfile : Profile
{
    public PersonaMapProfile()
    {
        CreateMap<Persona, PersonaDto>();
    }
}