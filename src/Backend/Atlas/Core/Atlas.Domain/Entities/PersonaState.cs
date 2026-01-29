using System.Text.Json.Serialization;
using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class PersonaState : BaseEntity
{
    public LifePhase CurrentPhase { get; private set; } = LifePhase.Execution;
    public MentalLoadLevel MentalLoad { get; private set; } = MentalLoadLevel.Medium;
    public int EnergyLevel { get; private set; } = 5;
    public int FocusLevel { get; private set; } = 5;
    public DateTime LastUpdatedAt { get; private set; } = DateTime.UtcNow;
    public string? Note { get; private set; } 
    public Guid PersonaId { get; private set; }
    
    [JsonIgnore]
    public Persona Persona { get; private set; } = null!;

    public static PersonaState Create(Guid personaId, LifePhase phase, MentalLoadLevel mentalLoad)
    {
        var personaState = new PersonaState
        {
            PersonaId = personaId,
            CurrentPhase = phase,
            MentalLoad = mentalLoad,
            LastUpdatedAt = DateTime.UtcNow
        };

        return personaState;
    }

    public void UpdatePhase(LifePhase newPhase, string? note = null)
    {
        CurrentPhase = newPhase;
        Note = note;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMentalLoad(MentalLoadLevel newLoad)
    {
        MentalLoad = newLoad;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEnergyLevel(int level)
    {
        EnergyLevel = level;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFocusLevel(int level)
    {
        FocusLevel = level;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public PersonaStateHistory ToHistory()
    {
        var history = PersonaStateHistory.CreateFrom(this);
        return history;
    }

}