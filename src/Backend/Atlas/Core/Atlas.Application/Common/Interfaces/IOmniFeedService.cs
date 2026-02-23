using Atlas.Domain.Enums;

namespace Atlas.Application.Common.Interfaces;

public interface IOmniFeedService
{
    Task<OmniFeedPage> GetFeedAsync(Guid teamId, OmniFeedSource? sourceFilter, int page, int pageSize, CancellationToken ct);
    Task MarkAsReadAsync(Guid itemId, CancellationToken ct);
    Task PublishManualItemAsync(Guid teamId, Guid userId, string title, string? body, CancellationToken ct);
    Task AddEmojiAsync(Guid itemId, string emoji, CancellationToken ct);
}

public record OmniFeedPage(List<OmniFeedItemDto> Items, int TotalCount, int Page, int PageSize);
public record OmniFeedItemDto(Guid Id, string Source, string Title, string? Body, string? UserName, DateTime Timestamp, bool IsRead, string? Emoji);

