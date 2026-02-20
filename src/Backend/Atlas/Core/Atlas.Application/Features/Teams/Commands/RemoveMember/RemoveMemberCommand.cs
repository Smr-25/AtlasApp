using MediatR;

namespace Atlas.Application.Features.Teams.Commands.RemoveMember;

public record RemoveMemberCommand(Guid TeamId, Guid UserId) : IRequest<bool>;

