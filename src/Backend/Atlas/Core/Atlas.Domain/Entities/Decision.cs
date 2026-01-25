using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class Decision : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; } 
    public DecisionStatus Status { get; private set;  }
}