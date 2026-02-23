using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.OmniFeed.Queries.GetOmniFeed;

public record GetOmniFeedQuery(Guid TeamId, OmniFeedSource? SourceFilter, int Page = 1, int PageSize = 20) : IRequest<OmniFeedPage>;

