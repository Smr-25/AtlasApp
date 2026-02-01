using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;

namespace Atlas.Application.MapProfiles;


public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<AppUser, AccountDto>()
            .ConstructUsing(src => new AccountDto(
                src.Id.ToString(),
                src.UserName ?? string.Empty,
                src.Email ?? string.Empty,
                src.FullName,
                src.PhoneNumber,
                src.EmailConfirmed,
                src.PhoneNumberConfirmed,
                src.CreatedAt,
                src.Status,
                src.LastLoginAt
            ));
    }
}
