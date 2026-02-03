using System.Diagnostics;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.System.Dtos;

namespace Atlas.Infrastructure.Services;

public class SystemMonitorService : ISystemMonitorService
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
}