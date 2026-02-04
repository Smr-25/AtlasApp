using Atlas.Application.Features.System.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IAiAdvisorService
{
    Task<AiHealthAdviceDto> AnalyzeHealthAsync(SystemSnapshotDto snapshot);
}