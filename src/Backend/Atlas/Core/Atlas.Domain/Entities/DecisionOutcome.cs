using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class DecisionOutcome : BaseEntity
{
    public Guid DecisionId { get; private set; }
    public OutcomeStatus Status { get; private set; }
    public string? Description { get; private set; }
    public DateTime RecordedAt { get; private set; } = DateTime.UtcNow;
    public bool WasExpected { get; private set; }
    public string? LessonLearned { get; private set; }
    public Decision Decision { get; private set; } = null!;

    public static DecisionOutcome Record(Guid decisionId, OutcomeStatus status,
        string? description = null, bool wasExpected = true, string? lessonLearned = null)
    {
        var outcome = new DecisionOutcome
        {
            DecisionId = decisionId,
            Status = status,
            Description = description,
            RecordedAt = DateTime.UtcNow,
            WasExpected = wasExpected,
            LessonLearned = lessonLearned
        };
        return outcome;
    }

}