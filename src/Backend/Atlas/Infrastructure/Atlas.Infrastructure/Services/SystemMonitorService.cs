using System.Diagnostics;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.System.Dtos;
using Hardware.Info;

namespace Atlas.Infrastructure.Services;

public class SystemMonitorService(IHardwareInfo hardwareInfo) : ISystemMonitorService
{
    private readonly Dictionary<string, string> _targetProcessNames = new()
    {
        { "rider", "JetBrains Rider" },
        { "idea", "IntelliJ IDEA" },
        { "devenv", "Visual Studio 2022" },
        { "Code", "VS Code" },
        { "Code - Insiders", "VS Code (Insiders)" },
        { "webstorm", "WebStorm" },
        { "datagrip", "DataGrip" },
        { "pycharm", "PyCharm" },
        { "clion", "CLion" },
        { "goland", "GoLand" },
        { "phpstorm", "PhpStorm" },
        { "androidstudio", "Android Studio" }
    };

    public Task<List<IdeStatusDto>> GetActiveIdesAsync(CancellationToken cancellationToken = default)
    {
        var activeIdes = new List<IdeStatusDto>();
        var allProcesses = Process.GetProcesses();

        foreach (var process in allProcesses)
        {

            var match = _targetProcessNames.FirstOrDefault(p =>
                p.Key.Equals(process.ProcessName, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(match.Key)) continue;

            try
            {

                var memoryMb = Math.Round(process.WorkingSet64 / 1024.0 / 1024.0, 1);

                activeIdes.Add(new IdeStatusDto(
                    Name: match.Value,
                    ProcessName: process.ProcessName,
                    MemoryUsageMb: memoryMb,
                    Uptime: DateTime.Now - process.StartTime,
                    IsResponding: process.Responding
                ));
            }
            catch
            {
                continue;
            }
        }

        return Task.FromResult(activeIdes);
    }

    public async Task<SystemSnapshotDto> GetSnapshotAsync()
    {
        await Task.Run(hardwareInfo.RefreshAll);

        var battery = hardwareInfo.BatteryList.FirstOrDefault();
        var batteryPercent = battery?.EstimatedChargeRemaining ?? 100;
        var status = battery?.BatteryStatus.ToString() ?? "Unknown";
        var remainingMin = battery != null ? (int)(battery.EstimatedRunTime / 60) : 0;

        var mem = hardwareInfo.MemoryStatus;
        var totalRam = mem.TotalPhysical / (1024.0 * 1024 * 1024);
        var availableRam = mem.AvailablePhysical / (1024.0 * 1024 * 1024);
        var usedRam = totalRam - availableRam;

        var cpu = hardwareInfo.CpuList.FirstOrDefault();
        var cpuLoad = cpu?.PercentProcessorTime ?? 0;

        var processes = Process.GetProcesses()
            .Select(p => new { Process = p, Mem = p.WorkingSet64 / (1024.0 * 1024) }) 
            .OrderByDescending(p => p.Mem)
            .Take(5) 
            .Select(p => new ProcessUsageDto(p.Process.ProcessName, p.Process.Id, Math.Round(p.Mem, 1)))
            .ToList();

        return new SystemSnapshotDto(
            batteryPercent,
            status,
            remainingMin,
            Math.Round((double)cpuLoad, 1),
            Math.Round(usedRam, 1),
            Math.Round(totalRam, 1),
            processes
        );
    }
}