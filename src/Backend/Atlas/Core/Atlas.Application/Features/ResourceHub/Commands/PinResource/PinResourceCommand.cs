using MediatR;

namespace Atlas.Application.Features.ResourceHub.Commands.PinResource;

public record PinResourceCommand(Guid ResourceId) : IRequest<Unit>;

