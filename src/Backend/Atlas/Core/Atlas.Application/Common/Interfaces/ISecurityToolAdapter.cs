using Atlas.Application.Features.SecurityTools.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface ISecurityToolAdapter
{
    Task<List<VulnerabilityReportDto>> ScanProjectAsync(string projectPath, CancellationToken ct);
}