using MediatR;

namespace Atlas.Application.Features.Teams.Commands.InviteMember;

public record InviteMemberCommand(Guid TeamId, Guid UserId) : IRequest<bool>;

