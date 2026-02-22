namespace Atlas.Application.Features.SonarQube.Dtos;

public record SonarQubeProjectQualityDto(
    string ProjectKey,
    string ProjectName,
    string ReliabilityGrade,
    string SecurityGrade,
    string MaintainabilityGrade,
    double CoveragePercent,
    int Bugs,
    int Vulnerabilities,
    int CodeSmells,
    int DuplicatedLines);

public record SonarQubeIssueDto(
    string Key,
    string Message,
    string Severity,
    string Component,
    int Line,
    string Type,
    string Status);

