using System.Text.Json.Serialization;
using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class Persona : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string? Alias { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastActiveAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? DeactivatedAt { get; private set; }
    [JsonIgnore] public PersonaState? CurrentState { get; private set; }

    [JsonIgnore]
    public ICollection<PersonaStateHistory> StateHistory { get; private set; } = new List<PersonaStateHistory>();

    [JsonIgnore] public ICollection<Decision> Decisions { get; private set; } = new List<Decision>();
    [JsonIgnore] public ICollection<Goal> Goals { get; private set; } = new List<Goal>();
    [JsonIgnore] public ICollection<Constraint> Constraints { get; private set; } = new List<Constraint>();

    [JsonIgnore]
    public ICollection<PersonaTimelineEvent> TimelineEvents { get; private set; } = new List<PersonaTimelineEvent>();

    public static Persona Create(Guid userId, string name, string? alias = null) => new() { Name = name, Alias = alias };
    public void UpdateName(string name) => Name = name;
    public void UpdateAlias(string? alias) => Alias = alias;
    public void Deactivate()
    {
        IsActive = false;
        DeactivatedAt = DateTime.UtcNow;
    }
    public void Activate()
    {
        IsActive = true;
        DeactivatedAt = null;
    }
    public void UpdateLastActive() => LastActiveAt = DateTime.UtcNow;
}