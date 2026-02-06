using MediatR;

namespace Atlas.Application.Features.Profiles.Commands.UpdateProfile;

public record UpdateUserProfileCommand(string JobTitle, string? Bio, string? ThemeColor) : IRequest;
