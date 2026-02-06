using Atlas.Application.Features.Workspaces.Dtos;
using Atlas.Domain.Entities;

namespace Atlas.Application.Features.Profiles.Dtos;

public record UserProfileDetailDto(
    Guid Id, 
    string JobTitle, 
    string Bio, 
    string ThemeColor, 
    string Profession,
    IReadOnlyCollection<Workspace> Workspaces
);