using Atlas.Application.Features.SecurityTools.Dtos;
using MediatR;

namespace Atlas.Application.Features.SecurityTools.Queries.ScanVulnerabilities;

public record ScanVulnerabilitiesQuery(string ProjectPath) : IRequest<List<VulnerabilityReportDto>>;