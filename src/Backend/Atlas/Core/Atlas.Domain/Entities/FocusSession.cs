using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class FocusSession : BaseEntity
{
    public int DurationMinutes { get; set; } 
    public string Tag { get; set; } = "Work"; 
    public DateTime CompletedAt { get; set; } 
    public Guid UserId { get; set; }
    
    public static FocusSession Create(int durationMinutes, string tag, Guid userId)
    {
        return new FocusSession
        {
            Id = Guid.NewGuid(),
            DurationMinutes = durationMinutes,
            Tag = tag,
            CompletedAt = DateTime.UtcNow,
            UserId = userId
        };
    }
}