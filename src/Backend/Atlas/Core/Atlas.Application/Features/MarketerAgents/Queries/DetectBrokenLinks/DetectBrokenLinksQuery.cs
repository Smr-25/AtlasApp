using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Queries.DetectBrokenLinks;

public record DetectBrokenLinksQuery(string BaseUrl) : IRequest<List<BrokenLinkResult>>;

