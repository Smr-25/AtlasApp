using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class ModalState : BaseEntity
{
    public Guid UserId { get; private set; }
    public ModalType ModalType { get; private set; }
    public bool HasBeenSeen { get; private set; }
    public DateTime? DismissedAt { get; private set; }
    public string? PayloadJson { get; private set; }

    private ModalState() { }

    public static ModalState Create(Guid userId, ModalType modalType, string? payloadJson = null)
    {
        return new ModalState
        {
            UserId = userId,
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
}

