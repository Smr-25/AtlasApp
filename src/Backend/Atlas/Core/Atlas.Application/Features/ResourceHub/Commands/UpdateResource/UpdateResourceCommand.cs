using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.ResourceHub.Commands.UpdateResource;

public record UpdateResourceCommand(Guid ResourceId, string Title, string Url, ResourceCategory Category, string? Description) : IRequest<Unit>;

