using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DesignUtilities.Queries.CalculateAspectRatio;

public record CalculateAspectRatioQuery(int Width, int Height) : IRequest<AspectRatioResult>;

