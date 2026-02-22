using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class SonarQubeReport : BaseEntity
{
    public string ProjectKey { get; private set; } = null!;
    public QualityGrade ReliabilityGrade { get; private set; }
    public QualityGrade SecurityGrade { get; private set; }
    public QualityGrade MaintainabilityGrade { get; private set; }
    public double CoveragePercent { get; private set; }
    public int TotalIssues { get; private set; }
    public int Bugs { get; private set; }
    public int Vulnerabilities { get; private set; }
    public int CodeSmells { get; private set; }
    public int DuplicatedLines { get; private set; }
    public Guid IntegrationId { get; private set; }
    public Guid UserId { get; private set; }

    private SonarQubeReport() { }

    public static SonarQubeReport Create(
        Guid userId,
        Guid integrationId,
        string projectKey,
        QualityGrade reliabilityGrade,
        QualityGrade securityGrade,
        QualityGrade maintainabilityGrade,
        double coveragePercent,
        int totalIssues,
        int bugs,
        int vulnerabilities,
        int codeSmells,
        int duplicatedLines)
    {
        return new SonarQubeReport
        {
            UserId = userId,
            IntegrationId = integrationId,
            ProjectKey = projectKey,
            ReliabilityGrade = reliabilityGrade,
            SecurityGrade = securityGrade,
            MaintainabilityGrade = maintainabilityGrade,
            CoveragePercent = coveragePercent,
            TotalIssues = totalIssues,
            Bugs = bugs,
            Vulnerabilities = vulnerabilities,
            CodeSmells = codeSmells,
            DuplicatedLines = duplicatedLines
        };
    }
}

