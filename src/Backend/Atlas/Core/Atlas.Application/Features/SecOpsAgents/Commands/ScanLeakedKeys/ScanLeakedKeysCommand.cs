using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Commands.ScanLeakedKeys;

public record ScanLeakedKeysCommand(string Content) : IRequest<List<LeakedKeyInfo>>;

