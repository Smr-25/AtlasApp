using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class LeaderModalState : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid? TeamId { get; private set; }
    public LeaderModalType ModalType { get; private set; }
    public bool HasBeenSeen { get; private set; }
    public DateTime? DismissedAt { get; private set; }
    public string? PayloadJson { get; private set; }

    private LeaderModalState() { }

    public static LeaderModalState Create(Guid userId, LeaderModalType modalType, Guid? teamId = null, string? payloadJson = null)
    {
        return new LeaderModalState
        {
            UserId = userId,
            TeamId = teamId,
            ModalType = modalType,
            HasBeenSeen = false,
            PayloadJson = payloadJson
        };
    }

    public void MarkAsSeen()
    {
        HasBeenSeen = true;
        SetModified();
    }

    public void Dismiss()
    {
        HasBeenSeen = true;
        DismissedAt = DateTime.UtcNow;
        SetModified();
    }

    public void UpdatePayload(string payloadJson)
    {
        PayloadJson = payloadJson;
        SetModified();
    }
}

