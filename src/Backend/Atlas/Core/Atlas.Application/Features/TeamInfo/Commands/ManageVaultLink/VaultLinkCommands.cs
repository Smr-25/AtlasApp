using MediatR;

namespace Atlas.Application.Features.TeamInfo.Commands.ManageVaultLink;

public record AddVaultLinkCommand(Guid TeamId, string Label, string Url, string? Icon, int SortOrder = 0) : IRequest<Guid>;

public record UpdateVaultLinkCommand(Guid TeamId, Guid LinkId, string Label, string Url, string? Icon, int SortOrder) : IRequest<Unit>;

public record DeleteVaultLinkCommand(Guid TeamId, Guid LinkId) : IRequest<Unit>;

