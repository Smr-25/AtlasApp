using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Queries.CalculateReadability;

public record CalculateReadabilityQuery(string Text) : IRequest<ReadabilityResult>;

