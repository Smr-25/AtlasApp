using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class DependencyWatch : BaseEntity
{
    public string PackageName { get; private set; } = null!;
    public string CurrentVersion { get; private set; } = null!;
    public string? LatestVersion { get; private set; }
    public bool IsOutdated { get; private set; }
    public bool HasVulnerability { get; private set; }
    public string? VulnerabilityDetail { get; private set; }
    public string ProjectPath { get; private set; } = null!;
    public Guid UserId { get; private set; }

    private DependencyWatch() { }

    public static DependencyWatch Create(
        Guid userId,
        string packageName,
        string currentVersion,
        string projectPath)
    {
        return new DependencyWatch
        {
            UserId = userId,
            PackageName = packageName,
            CurrentVersion = currentVersion,
            ProjectPath = projectPath,
            IsOutdated = false,
            HasVulnerability = false
        };
    }

    public void UpdateLatestVersion(string latestVersion)
    {
        LatestVersion = latestVersion;
        IsOutdated = latestVersion != CurrentVersion;
        SetModified();
    }

    public void FlagVulnerability(string detail)
    {
        HasVulnerability = true;
        VulnerabilityDetail = detail;
        SetModified();
    }

    public void ClearVulnerability()
    {
        HasVulnerability = false;
        VulnerabilityDetail = null;
        SetModified();
    }
}

