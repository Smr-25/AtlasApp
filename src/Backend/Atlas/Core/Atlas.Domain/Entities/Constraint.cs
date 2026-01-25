using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class Constraint : Persona
{
    public string Type { get; private set; } = null!;
    public string Description { get; private set; } = null!;
}