using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.UpdateLinkConfig;

public record UpdateLinkConfigCommand(
    Guid LinkId,              
    object ConfigData         
) : IRequest<bool>;