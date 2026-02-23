using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Queries.ScanLocalPorts;

public record ScanLocalPortsQuery(string Target = "127.0.0.1", int StartPort = 1, int EndPort = 1024) : IRequest<List<OpenPortResult>>;

