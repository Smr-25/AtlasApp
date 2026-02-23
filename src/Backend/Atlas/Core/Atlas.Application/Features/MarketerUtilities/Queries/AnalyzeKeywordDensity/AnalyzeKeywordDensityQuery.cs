using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Queries.AnalyzeKeywordDensity;

public record AnalyzeKeywordDensityQuery(string Content, string Keyword) : IRequest<KeywordDensityResult>;

