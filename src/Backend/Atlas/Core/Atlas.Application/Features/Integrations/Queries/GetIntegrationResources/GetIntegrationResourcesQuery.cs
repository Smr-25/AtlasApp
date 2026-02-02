using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Integrations.Queries.GetIntegrationResources;

public record GetIntegrationResourcesQuery(Guid IntegrationId) : IRequest<List<ExternalResourceDto>>;