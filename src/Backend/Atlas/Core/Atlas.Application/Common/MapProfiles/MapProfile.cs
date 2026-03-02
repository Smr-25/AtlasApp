using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Application.Features.Integrations.Dtos;
using Atlas.Application.Features.Profiles.Dtos;
using Atlas.Application.Features.Workspaces.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;

namespace Atlas.Application.Common.MapProfiles;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<AppUser, AccountDto>();
        
        CreateMap<AppUserProfile, UserProfileDetailDto>()
            .ForMember(d => d.Profession, opt => opt.MapFrom(s => s.Profession.ToString()));

        CreateMap<Integration, IntegrationDto>();

        CreateMap<WorkspaceIntegration, WorkspaceIntegrationDto>()
            .ForMember(d => d.IntegrationId, opt => opt.MapFrom(s => s.IntegrationId))
            .ForMember(d => d.IntegrationName, opt => opt.MapFrom(s => s.Integration.Name))
            .ForMember(d => d.Provider, opt => opt.MapFrom(s => s.Integration.Provider))
            .ForMember(d => d.Scope, opt => opt.MapFrom(s => s.Integration.Scope))
            .ForMember(d => d.Enabled, opt => opt.MapFrom(s => s.Enabled))
            .ForMember(d => d.ConnectedAt, opt => opt.MapFrom(s => s.CreatedAt));

        CreateMap<Workspace, WorkspaceDto>()
            .ForMember(d => d.ActiveIntegrations, opt => opt.MapFrom(s => 
                s.WorkspaceIntegrations.Where(wi => wi.Enabled)))
            .ForMember(d => d.LocalFolderPath, opt => opt.MapFrom(s => s.LocalFolderPath))
            .ForMember(d => d.IsShared, opt => opt.MapFrom(s => s.IsShared))
            .ForMember(d => d.MembersCount, opt => opt.MapFrom(s => s.Members.Count))
            .ForMember(d => d.MyRole, opt => opt.Ignore()); // Set manually in handler
    }
}