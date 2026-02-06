using Atlas.Application.Features.Profiles.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Profiles.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDetailDto>;