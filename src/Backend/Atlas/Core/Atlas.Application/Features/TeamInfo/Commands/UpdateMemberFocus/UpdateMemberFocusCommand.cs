using MediatR;

namespace Atlas.Application.Features.TeamInfo.Commands.UpdateMemberFocus;

public record UpdateMemberFocusCommand(
    Guid TeamId,
    string FocusDescription
) : IRequest<Guid>;

