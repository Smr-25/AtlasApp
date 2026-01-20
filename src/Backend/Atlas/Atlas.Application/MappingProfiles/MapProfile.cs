using Atlas.Application.Dtos.Users;
using Atlas.Domain.Entities;
using AutoMapper;

namespace Atlas.Application.MappingProfiles;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<UserRegisterDto, AppUser>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? string.Empty))
            .ForAllMembers(opt => opt.Ignore());
            
    }
}