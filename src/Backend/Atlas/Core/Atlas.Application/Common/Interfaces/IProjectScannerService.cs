using Atlas.Application.Features.Projects.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IProjectScannerService
{
    Task<List<LocalProjectDto>> ScanForProjectsAsync(string rootPath, CancellationToken ct);
}