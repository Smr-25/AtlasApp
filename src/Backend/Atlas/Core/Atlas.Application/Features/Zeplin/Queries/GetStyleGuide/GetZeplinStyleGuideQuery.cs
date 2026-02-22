using Atlas.Application.Features.Zeplin.Dtos;
using MediatR;

namespace Atlas.Application.Features.Zeplin.Queries.GetStyleGuide;

public record GetZeplinStyleGuideQuery(Guid IntegrationId, string ProjectId) : IRequest<ZeplinStyleGuideDto>;

