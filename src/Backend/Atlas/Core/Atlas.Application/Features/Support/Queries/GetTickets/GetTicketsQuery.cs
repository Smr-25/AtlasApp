using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Support.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Support.Queries.GetTickets;

public record GetTicketsQuery : IRequest<List<SupportTicketDto>>;

public class GetTicketsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<GetTicketsQuery, List<SupportTicketDto>>
{
    public async Task<List<SupportTicketDto>> Handle(GetTicketsQuery request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        return await context.SupportTickets
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new SupportTicketDto(
                t.Id, t.Type, t.Status, t.Subject, t.Body,
                t.PageUrl, t.AdminReply, t.RepliedAt, t.CreatedAt))
            .ToListAsync(ct);
    }
}

