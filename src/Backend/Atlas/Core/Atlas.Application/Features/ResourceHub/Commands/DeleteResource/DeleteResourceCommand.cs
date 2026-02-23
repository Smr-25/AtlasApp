using MediatR;

namespace Atlas.Application.Features.ResourceHub.Commands.DeleteResource;

public record DeleteResourceCommand(Guid ResourceId) : IRequest<Unit>;

