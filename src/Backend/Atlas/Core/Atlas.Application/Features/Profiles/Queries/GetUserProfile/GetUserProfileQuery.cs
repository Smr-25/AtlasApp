using Atlas.Application.Features.Profiles.Dtos;
using MediatR;

namespace Atlas.Application.Features.Profiles.Queries.GetUserProfile;

public record GetUserProfileQuery : IRequest<UserProfileDetailDto>;
