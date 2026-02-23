using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Services;

public class SquadRadarService(IApplicationDbContext dbContext) : ISquadRadarService
{
    public async Task<SquadRadarSnapshot> GetRadarSnapshotAsync(Guid teamId, CancellationToken ct)
    {
        var entries = await dbContext.SquadRadarEntries
            .Where(e => e.TeamId == teamId)
            .ToListAsync(ct);

        var members = entries.Select(e => new RadarMemberInfo(
            e.UserId,
            string.Empty,
            null,
            e.OnlineStatus,
            e.CurrentToolIcon,
            e.CurrentFocus,
            e.MeetingMinutesLeft,
            e.LastActiveAt
        )).ToList();

        return new SquadRadarSnapshot(teamId, members);
    }

    public async Task UpdatePresenceAsync(Guid userId, Guid teamId, SquadMemberStatus status, string? toolIcon, string? focus, int? meetingMinutesLeft, CancellationToken ct)
    {
        var entry = await dbContext.SquadRadarEntries
            .FirstOrDefaultAsync(e => e.TeamId == teamId && e.UserId == userId, ct);

        if (entry == null)
        {
            entry = Atlas.Domain.Entities.SquadRadarEntry.Create(teamId, userId);
            entry.UpdatePresence(status, toolIcon, focus, meetingMinutesLeft);
            dbContext.SquadRadarEntries.Add(entry);
        }
        else
        {
            entry.UpdatePresence(status, toolIcon, focus, meetingMinutesLeft);
        }

        await dbContext.SaveChangesAsync(ct);
    }
}

