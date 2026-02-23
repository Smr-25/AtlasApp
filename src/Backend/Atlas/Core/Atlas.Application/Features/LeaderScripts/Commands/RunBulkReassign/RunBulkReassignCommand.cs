using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunBulkReassign;

public record RunBulkReassignCommand(Guid AbsentMemberId, Guid TeamId) : IRequest<BulkReassignResult>;

