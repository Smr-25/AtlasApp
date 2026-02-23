using MediatR;

namespace Atlas.Application.Features.SecOpsScripts.Commands.RunQuickScan;

public record RunQuickScanCommand(string NetworkRange) : IRequest<string>;

