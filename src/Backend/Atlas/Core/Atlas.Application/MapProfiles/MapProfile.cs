using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Application.Features.Personas.Dtos;
using Atlas.Application.Features.Snippets.Dtos;
using Atlas.Application.Features.Workspaces.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;

namespace Atlas.Application.MapProfiles;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<AppUser, AccountDto>();

        CreateMap<Persona, PersonaDto>()
            .ForMember(dest => dest.Integrations, opt => opt.MapFrom(src => src.Integrations.Where(i => !i.IsDeleted)));
        
        CreateMap<Persona, PersonaDetailDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Integrations, opt => opt.MapFrom(src => src.Integrations.Where(i => !i.IsDeleted)))
            .ForMember(dest => dest.Workspaces, opt => opt.MapFrom(src => src.Workspaces.Where(w => !w.IsDeleted)));
        
        CreateMap<Integration, IntegrationDto>()
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider.ToString()));
        
        CreateMap<Integration, PersonaIntegrationDto>()
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider.ToString()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        
        CreateMap<Workspace, PersonaWorkspaceDto>();

        CreateMap<Snippet, SnippetDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.Tags) 
                    ? Array.Empty<string>() 
                    : src.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)));

        CreateMap<Workspace, WorkspaceDto>();
    }
}