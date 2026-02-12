using Atlas.Application.Features.SystemTools.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface ISystemToolAdapter
{
    Task<ProcessInfoDto> GetProcessByPortAsync(int port, CancellationToken ct);

    Task KillProcessAsync(int pid, CancellationToken ct);
}