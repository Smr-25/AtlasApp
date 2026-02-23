using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.ResourceHub.Queries.GetResources;

public record GetResourcesQuery(Guid TeamId, ResourceCategory? CategoryFilter = null) : IRequest<List<SharedResourceDto>>;

public record SharedResourceDto(Guid Id, string Title, string? Description, string Url, string Category, bool IsPinned, Guid UploadedByUserId);

