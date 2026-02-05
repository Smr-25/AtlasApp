using Atlas.Domain.Events;

namespace Atlas.Domain.Entities.Common;

public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; protected init; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; protected init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedAt { get; protected set; }
    public bool IsDeleted { get; private set; }
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void SetModified()
    {
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public virtual void Delete()
    {
        IsDeleted = true;
        SetModified();
    }

    public virtual void Restore()
    {
        IsDeleted = false;
        SetModified();
    }
}