using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.SquadArena.Commands.AwardBadge;

public record AwardBadgeCommand(Guid TeamId, Guid UserId, ArenaBadgeType BadgeType, int Points, string? SprintId) : IRequest<Guid>;

