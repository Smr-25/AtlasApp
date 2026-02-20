using MediatR;

namespace Atlas.Application.Features.Subscriptions.Commands.CreatePortalSession;

public record CreatePortalSessionCommand(string ReturnUrl) : IRequest<string>;

