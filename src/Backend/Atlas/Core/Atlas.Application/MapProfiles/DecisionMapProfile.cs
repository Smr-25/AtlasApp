using Atlas.Application.Features.Decisions.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;

namespace Atlas.Application.MapProfiles;

public class DecisionMapProfile : Profile
{
    public DecisionMapProfile()
    {
        CreateMap<Decision, DecisionDto>();
    }
}