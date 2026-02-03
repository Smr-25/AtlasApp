using Atlas.Application.Features.Snippets.Dtos;
using MediatR;

namespace Atlas.Application.Features.Snippets.Queries.GetSnippets;

public record GetSnippetsQuery : IRequest<List<SnippetDto>>;