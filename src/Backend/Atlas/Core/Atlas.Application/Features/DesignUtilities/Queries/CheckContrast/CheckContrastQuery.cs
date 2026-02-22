using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DesignUtilities.Queries.CheckContrast;

public record CheckContrastQuery(string ForegroundHex, string BackgroundHex) : IRequest<ContrastCheckResult>;

