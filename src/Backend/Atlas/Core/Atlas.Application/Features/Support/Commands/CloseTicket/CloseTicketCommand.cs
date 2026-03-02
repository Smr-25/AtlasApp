using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Support.Commands.CloseTicket;

public record CloseTicketCommand(Guid TicketId) : IRequest;

public class CloseTicketCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<CloseTicketCommand>
{
    public async Task Handle(CloseTicketCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var ticket = await context.SupportTickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.UserId == userId, ct)
            ?? throw new NotFoundException("Ticket", request.TicketId);

        ticket.Close();
        await context.SaveChangesAsync(ct);
    }
}

