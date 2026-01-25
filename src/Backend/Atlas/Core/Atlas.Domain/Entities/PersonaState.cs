using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class PersonaState : Persona
{
    public string CurrentPhase { get; private set; } = "Exploring";
    public string MentalLoadLevel { get; private set; } = "Medium";
    public DateTime LastUpdatedAt { get; private set; }
    public int PersonaId { get; private set; }
}