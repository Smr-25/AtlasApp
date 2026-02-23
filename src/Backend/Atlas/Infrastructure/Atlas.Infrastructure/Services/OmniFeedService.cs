using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Services;

public class OmniFeedService(IApplicationDbContext dbContext) : IOmniFeedService
{
    public async Task<OmniFeedPage> GetFeedAsync(Guid teamId, OmniFeedSource? sourceFilter, int page, int pageSize, CancellationToken ct)
    {
        var query = dbContext.OmniFeedItems.Where(f => f.TeamId == teamId);

        if (sourceFilter.HasValue)
            query = query.Where(f => f.Source == sourceFilter.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(f => f.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new OmniFeedItemDto(
                f.Id,
                f.Source.ToString(),
                f.Title,
                f.Body,
                null,
                f.Timestamp,
                f.IsRead,
                f.Emoji
            ))
            .ToListAsync(ct);

        return new OmniFeedPage(items, totalCount, page, pageSize);
    }

    public async Task MarkAsReadAsync(Guid itemId, CancellationToken ct)
    {
        var item = await dbContext.OmniFeedItems.FirstOrDefaultAsync(f => f.Id == itemId, ct);
        if (item != null)
        {
            item.MarkAsRead();
            await dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task PublishManualItemAsync(Guid teamId, Guid userId, string title, string? body, CancellationToken ct)
    {
        var item = OmniFeedItem.Create(teamId, OmniFeedSource.Manual, title, body, userId);
        dbContext.OmniFeedItems.Add(item);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task AddEmojiAsync(Guid itemId, string emoji, CancellationToken ct)
    {
        var item = await dbContext.OmniFeedItems.FirstOrDefaultAsync(f => f.Id == itemId, ct);
        if (item != null)
        {
            item.AddEmoji(emoji);
            await dbContext.SaveChangesAsync(ct);
        }
    }
}

