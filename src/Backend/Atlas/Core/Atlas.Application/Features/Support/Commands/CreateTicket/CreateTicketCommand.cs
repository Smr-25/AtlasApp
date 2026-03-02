using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Support.Commands.CreateTicket;

public record CreateTicketCommand(
    FeedbackType Type,
    string Subject,
    string Body,
    string? PageUrl = null,
    string? BrowserInfo = null
) : IRequest<Guid>;

public class CreateTicketCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<CreateTicketCommand, Guid>
{
    public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var ticket = SupportTicket.Create(userId, request.Type, request.Subject, request.Body, request.PageUrl, request.BrowserInfo);
        await context.SupportTickets.AddAsync(ticket, ct);
        await context.SaveChangesAsync(ct);
        return ticket.Id;
    }
}

