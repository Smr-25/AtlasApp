using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Commands.PingGhostMembers;

public record PingGhostMembersCommand(Guid TeamId) : IRequest<GhostMemberResult>;

