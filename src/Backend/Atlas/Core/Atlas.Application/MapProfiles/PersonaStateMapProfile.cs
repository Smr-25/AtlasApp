using Atlas.Application.Features.PersonaStates.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;

namespace Atlas.Application.MapProfiles;

public class PersonaStateMapProfile : Profile
{
    public PersonaStateMapProfile()
    {
        CreateMap<PersonaState, PersonaStateDto>();
    }
}