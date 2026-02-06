using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Application.Features.Integrations.Dtos;
using Atlas.Application.Features.Onboarding.Dtos;
using Atlas.Application.Features.Profiles.Dtos;
using Atlas.Application.Features.Snippets.Dtos;
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

        CreateMap<Workspace, WorkspaceDto>();
        CreateMap<Integration, IntegrationDto>();
    }
}