namespace Atlas.Application.Features.System.Dtos;

public record SystemSnapshotDto(
    int BatteryPercentage,
    string BatteryStatus,    
    int RemainingMinutes,    
    double CpuLoad,          
    double MemoryUsedGb,    
    double TotalMemoryGb,    
    List<ProcessUsageDto> TopProcesses 
);