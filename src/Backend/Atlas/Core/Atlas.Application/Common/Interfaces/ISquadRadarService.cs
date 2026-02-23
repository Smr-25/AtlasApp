using Atlas.Domain.Enums;

namespace Atlas.Application.Common.Interfaces;

public interface ISquadRadarService
{
    Task<SquadRadarSnapshot> GetRadarSnapshotAsync(Guid teamId, CancellationToken ct);
    Task UpdatePresenceAsync(Guid userId, Guid teamId, SquadMemberStatus status, string? toolIcon, string? focus, int? meetingMinutesLeft, CancellationToken ct);
}

public record SquadRadarSnapshot(Guid TeamId, List<RadarMemberInfo> Members);
public record RadarMemberInfo(Guid UserId, string DisplayName, string? AvatarUrl, SquadMemberStatus Status, string? CurrentToolIcon, string? CurrentFocus, int? MeetingMinutesLeft, DateTime LastActiveAt);

