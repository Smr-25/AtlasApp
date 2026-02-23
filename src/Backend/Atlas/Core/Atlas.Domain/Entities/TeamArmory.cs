using Atlas.Domain.Entities.Common;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class TeamArmory : BaseEntity
{
    public Guid TeamId { get; private set; }
    public string StagingServerUrl { get; private set; } = null!;
    public bool IsStagingOnline { get; private set; }
    public string? TestAccountEmail { get; private set; }
    public string? TestAccountPassword { get; private set; }
    public string? ProductionVersion { get; private set; }
    public string? StagingVersion { get; private set; }

    private TeamArmory() { }

    public static TeamArmory Create(
        Guid teamId,
        string stagingServerUrl,
        string? testAccountEmail = null,
        string? testAccountPassword = null,
        string? productionVersion = null)
    {
        if (string.IsNullOrWhiteSpace(stagingServerUrl))
            throw new InvalidEntityStateException(nameof(TeamArmory), nameof(StagingServerUrl), "Staging server URL cannot be empty.");

        return new TeamArmory
        {
            TeamId = teamId,
            StagingServerUrl = stagingServerUrl.Trim(),
            IsStagingOnline = false,
            TestAccountEmail = testAccountEmail?.Trim(),
            TestAccountPassword = testAccountPassword?.Trim(),
            ProductionVersion = productionVersion?.Trim()
        };
    }

    public void UpdateStagingStatus(bool isOnline, string? stagingVersion = null)
    {
        IsStagingOnline = isOnline;
        if (stagingVersion != null)
            StagingVersion = stagingVersion.Trim();
        SetModified();
    }

    public void UpdateTestAccount(string email, string password)
    {
        TestAccountEmail = email.Trim();
        TestAccountPassword = password.Trim();
        SetModified();
    }

    public void UpdateVersions(string? productionVersion, string? stagingVersion)
    {
        if (productionVersion != null) ProductionVersion = productionVersion.Trim();
        if (stagingVersion != null) StagingVersion = stagingVersion.Trim();
        SetModified();
    }
}

