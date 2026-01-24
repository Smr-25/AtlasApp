using Atlas.Application.Dtos.Users;
using Atlas.Application.Dtos.Users.Profile;
using Atlas.Domain.Entities;
using AutoMapper;

namespace Atlas.Application.MappingProfiles;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<AppUser, UserProfileReturnDto>();
        
        CreateMap<UserProfileUpdateDto, AppUser>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}