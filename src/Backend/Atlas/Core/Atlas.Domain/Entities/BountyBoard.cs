using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class BountyBoard : BaseEntity
{
    public Guid TeamId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public int RewardPoints { get; private set; }
    public Guid? ClaimedByUserId { get; private set; }
    public bool IsCompleted { get; private set; }
    public string? JiraIssueKey { get; private set; }

    private BountyBoard() { }

    public static BountyBoard Create(Guid teamId, string title, int rewardPoints, string? description = null, string? jiraIssueKey = null)
    {
        return new BountyBoard
        {
            TeamId = teamId,
            Title = title,
            Description = description,
            RewardPoints = rewardPoints,
            JiraIssueKey = jiraIssueKey,
            IsCompleted = false
        };
    }

    public void Claim(Guid userId)
    {
        ClaimedByUserId = userId;
        SetModified();
    }

    public void Complete()
    {
        IsCompleted = true;
        SetModified();
    }
}

