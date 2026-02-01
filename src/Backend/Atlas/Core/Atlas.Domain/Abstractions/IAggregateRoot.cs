namespace Atlas.Domain.Abstractions;

public interface IAggregateRoot
{
    Guid Id { get; }
}