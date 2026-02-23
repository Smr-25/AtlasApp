using Atlas.Domain.Entities.Common;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class TeamVaultLink : BaseEntity
{
    public Guid TeamId { get; private set; }
    public string Label { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public string? Icon { get; private set; }
    public int SortOrder { get; private set; }

    private TeamVaultLink() { }

    public static TeamVaultLink Create(Guid teamId, string label, string url, string? icon = null, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new InvalidEntityStateException(nameof(TeamVaultLink), nameof(Label), "Link label cannot be empty.");
        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidEntityStateException(nameof(TeamVaultLink), nameof(Url), "Link URL cannot be empty.");

        return new TeamVaultLink
        {
            TeamId = teamId,
            Label = label.Trim(),
            Url = url.Trim(),
            Icon = icon?.Trim(),
            SortOrder = sortOrder
        };
    }

    public void Update(string label, string url, string? icon, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new InvalidEntityStateException(nameof(TeamVaultLink), nameof(Label), "Link label cannot be empty.");

        Label = label.Trim();
        Url = url.Trim();
        Icon = icon?.Trim();
        SortOrder = sortOrder;
        SetModified();
    }

    public void Delete()
    {
        SetDelete();
    }
}

