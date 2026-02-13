using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.SecurityTools.Dtos;
using MediatR;

namespace Atlas.Application.Features.SecurityTools.Queries.ScanVulnerabilities;

public class ScanVulnerabilitiesQueryHandler(ISecurityToolAdapter securityTool) 
    : IRequestHandler<ScanVulnerabilitiesQuery, List<VulnerabilityReportDto>>
{
    public async Task<List<VulnerabilityReportDto>> Handle(ScanVulnerabilitiesQuery request, CancellationToken cancellationToken)
    {
        return await securityTool.ScanProjectAsync(request.ProjectPath, cancellationToken);
    }
}