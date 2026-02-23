using MediatR;

namespace Atlas.Application.Features.OmniFeed.Commands.PublishManualItem;

public record PublishManualItemCommand(Guid TeamId, string Title, string? Body) : IRequest<Unit>;

