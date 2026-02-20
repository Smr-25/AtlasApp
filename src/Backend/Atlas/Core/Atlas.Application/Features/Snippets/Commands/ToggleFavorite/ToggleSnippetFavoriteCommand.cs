using MediatR;

namespace Atlas.Application.Features.Snippets.Commands.ToggleFavorite;

public record ToggleSnippetFavoriteCommand(Guid SnippetId) : IRequest<bool>;

