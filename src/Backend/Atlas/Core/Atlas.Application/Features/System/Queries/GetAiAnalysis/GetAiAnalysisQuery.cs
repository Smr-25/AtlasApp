using Atlas.Application.Features.System.Dtos;
using MediatR;

namespace Atlas.Application.Features.System.Queries.GetAiAnalysis;

public record SystemAnalysisResult(SystemSnapshotDto Snapshot, AiHealthAdviceDto Advice);

public record GetAiAnalysisQuery : IRequest<SystemAnalysisResult>;