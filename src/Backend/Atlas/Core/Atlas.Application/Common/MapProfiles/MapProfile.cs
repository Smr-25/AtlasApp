using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Application.Features.Onboarding.Dtos;
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
        
        CreateMap<OnboardingQuestion, OnboardingQuestionDto>()
            .ForMember(d => d.Options, opt => opt.MapFrom(s => s.Options));
        CreateMap<OnboardingOption, OnboardingOptionDto>();

        CreateMap<Workspace, WorkspaceDto>()
            .ForMember(d => d.ActiveIntegrations, opt => opt.MapFrom(s => 
                s.WorkspaceIntegrations.Where(wi => wi.Enabled).Select(wi => wi.Integration)))
            .ForMember(d => d.LocalFolderPath, opt => opt.MapFrom(s => s.LocalFolderPath))
            .ForMember(d => d.IsShared, opt => opt.MapFrom(s => s.IsShared));
    }
}