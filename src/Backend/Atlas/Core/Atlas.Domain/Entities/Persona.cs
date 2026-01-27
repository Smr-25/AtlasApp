using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class Persona : BaseEntity
{
   public string Name { get; set; } = null!;
   public string? Alias { get; private set; }
   public bool IsActive { get; private set; } = true;
   public DateTime? LastActiveAt { get; private set; }
   public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
   public DateTime? DeactivatedAt { get; private set; }
   public PersonaState? CurrentState { get; private set; } 
   public ICollection<PersonaStateHistory> StateHistory { get; private set; } = new List<PersonaStateHistory> { };
   public ICollection<Decision> Decisions { get; private set; } = new List<Decision>();
   public ICollection<Goal> Goals { get; private set; } = new List<Goal>();
   public ICollection<Constraint> Constraints { get; private set; } = new List<Constraint>();
   public ICollection<PersonaTimelineEvent> TimelineEvents { get; private set; } = new List<PersonaTimelineEvent>();
   public static Persona Create(Guid userId, string name, string? alias = null)
   {
      var persona = new Persona
      {
         Name = name,
         Alias = alias
      };
      return persona;
   }
   
   public void UpdateName(string name)
   {
      Name = name;
   }

   public void UpdateAlias(string? alias)
   {
      Alias = alias;
   }

   public void Deactivate()
   {
      IsActive = false;
   }

   public void Activate()
   {
      IsActive = true;
   }

   public void UpdateLastActive()
   {
      LastActiveAt = DateTime.UtcNow;
   }
}