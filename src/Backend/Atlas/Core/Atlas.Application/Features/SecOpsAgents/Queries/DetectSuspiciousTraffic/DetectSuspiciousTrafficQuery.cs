using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Queries.DetectSuspiciousTraffic;

public record DetectSuspiciousTrafficQuery(string TargetUrl) : IRequest<TrafficAnalysisResult>;

