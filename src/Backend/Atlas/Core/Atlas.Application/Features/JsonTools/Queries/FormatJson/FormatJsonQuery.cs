using MediatR;

namespace Atlas.Application.Features.JsonTools.Queries.FormatJson;

public record FormatJsonQuery(string JsonContent, bool Minify = false) : IRequest<string>;