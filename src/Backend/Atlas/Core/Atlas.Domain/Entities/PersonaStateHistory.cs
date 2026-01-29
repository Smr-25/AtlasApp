using System.Text.Json.Serialization;
using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class PersonaStateHistory : BaseEntity
{
    public LifePhase Phase { get; private set; } 
    public MentalLoadLevel MentalLoad { get; private set; }
    public int EnergyLevel { get; private set; }
    public int FocusLevel { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime EndedAt { get; private set; }
    public int DurationDays => (EndedAt - StartedAt).Days;
    public string? Note { get; private set; }
    public Guid PersonaId { get; private set; }
    
    [JsonIgnore]
    public Persona Persona { get; private set; } = null!;
    public static PersonaStateHistory CreateFrom(PersonaState state)
    {
        var history = new PersonaStateHistory
        {
            PersonaId = state.PersonaId,
            Phase = state.CurrentPhase,
            MentalLoad = state.MentalLoad,
            EnergyLevel = state.EnergyLevel,
            FocusLevel = state.FocusLevel,
            StartedAt = state.LastUpdatedAt,
            EndedAt = DateTime.UtcNow,
            Note = state.Note
        };

        return history;
    }
}