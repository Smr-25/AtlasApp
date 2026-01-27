namespace Atlas.Domain.Entities.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public Guid UserId { get; protected set; }
    
    protected void SetUserId(Guid userId) => UserId = userId;
}