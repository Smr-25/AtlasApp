using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Queries.CheckSeoMeta;

public record CheckSeoMetaQuery(string Title, string Description, string Url) : IRequest<SeoMetaCheckResult>;

