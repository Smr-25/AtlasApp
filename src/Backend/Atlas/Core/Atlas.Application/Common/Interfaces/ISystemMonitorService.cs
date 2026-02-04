using Atlas.Application.Features.System.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface ISystemMonitorService
{
    Task<List<IdeStatusDto>> GetActiveIdesAsync(CancellationToken cancellationToken = default);
    Task<SystemSnapshotDto> GetSnapshotAsync();
}