using Atlas.Application.Features.SonarQube.Dtos;
using MediatR;

namespace Atlas.Application.Features.SonarQube.Queries.GetProjectQuality;

public record GetProjectQualityQuery(Guid IntegrationId, string ProjectKey) : IRequest<SonarQubeProjectQualityDto>;

