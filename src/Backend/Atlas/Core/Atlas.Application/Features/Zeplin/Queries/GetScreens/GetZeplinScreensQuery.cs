using Atlas.Application.Features.Zeplin.Dtos;
using MediatR;

namespace Atlas.Application.Features.Zeplin.Queries.GetScreens;

public record GetZeplinScreensQuery(Guid IntegrationId, string ProjectId) : IRequest<List<ZeplinScreenDto>>;

