using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class Constraint : Persona
{
    public ConstraintType Type { get; private set; }
    public string Description { get; private set; } = null!;
    public int ImpactLevel { get; private set; } = 5;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Guid PersonaId { get; private set; }
    public Persona Persona { get; private set; } = null!;

    public static Constraint Create(Guid personaId, ConstraintType type, string description,
        int impactLevel = 5, DateTime? expiresAt = null)
    {
        var constraint = new Constraint
        {
            PersonaId = personaId,
            Type = type,
            Description = description,
            ImpactLevel = impactLevel,
            CreatedAt = DateTime.UtcNow
        };
        return constraint;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateImpactLevel(int level)
    {
        ImpactLevel = level;
    }
}