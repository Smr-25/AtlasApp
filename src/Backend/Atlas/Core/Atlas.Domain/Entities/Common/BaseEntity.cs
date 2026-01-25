namespace Atlas.Domain.Entities.Common;

public abstract class BaseEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
}