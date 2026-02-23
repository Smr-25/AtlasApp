using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.ResourceHub.Commands.AddResource;

public record AddResourceCommand(Guid TeamId, string Title, string Url, ResourceCategory Category, string? Description) : IRequest<Guid>;

