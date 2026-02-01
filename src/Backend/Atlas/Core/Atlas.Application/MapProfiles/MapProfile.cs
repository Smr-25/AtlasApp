using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Application.Features.Personas.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;

namespace Atlas.Application.MapProfiles;

public class MapProfile : Profile
{
    public  MapProfile()
    {
        CreateMap<AppUser,AccountDto>();
        CreateMap<Persona, PersonaDto>()
            .ForMember(dest => dest.Integrations, opt => opt.MapFrom(src => src.Integrations));
        CreateMap<Integration, IntegrationDto>()
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider.ToString()));
    }
}