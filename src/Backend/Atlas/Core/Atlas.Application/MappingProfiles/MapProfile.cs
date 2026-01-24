using Atlas.Application.Dtos.Users;
using Atlas.Domain.Entities;
using AutoMapper;

namespace Atlas.Application.MappingProfiles;

public class MapProfile : Profile
{
    public MapProfile()
    {
        // Note: UserRegisterDto → AppUser mapping removed
        // Manual mapping via AppUser.Create() factory method is used in AccountService.RegisterAsync
        // because AppUser has private setters and requires specific initialization
        
        // Add future mappings here when needed
        // Example: CreateMap<AppUser, UserProfileDto>();
    }
}