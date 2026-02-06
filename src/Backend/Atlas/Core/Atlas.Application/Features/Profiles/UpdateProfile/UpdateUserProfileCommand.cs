using MediatR;

namespace Atlas.Application.Features.Profiles.UpdateProfile;

public record UpdateUserProfileCommand(Guid UserId, string JobTitle, string? Bio, string? ThemeColor) : IRequest;