using MediatR;

namespace Atlas.Application.Features.Miro.Commands.CreateSticky;

public record CreateMiroStickyCommand(Guid IntegrationId, string BoardId, string Content) : IRequest;

