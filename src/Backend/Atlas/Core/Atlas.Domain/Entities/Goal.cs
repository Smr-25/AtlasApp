using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class Goal : Persona
{
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public GoalStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
}