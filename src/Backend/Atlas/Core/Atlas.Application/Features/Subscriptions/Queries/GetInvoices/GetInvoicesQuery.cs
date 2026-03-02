using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Subscriptions.Queries.GetInvoices;

public record InvoiceDto(
    string Id,
    DateTime Date,
    string Status,
    long AmountPaid,
    string Currency,
    string? PdfUrl,
    string? HostedUrl
);

public record GetInvoicesQuery : IRequest<List<InvoiceDto>>;

public class GetInvoicesQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IStripeService stripeService) : IRequestHandler<GetInvoicesQuery, List<InvoiceDto>>
{
    public async Task<List<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        var subscription = await context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.StripeCustomerId != null, ct);

        if (subscription?.StripeCustomerId == null)
            return [];

        return await stripeService.GetInvoicesAsync(subscription.StripeCustomerId, ct);
    }
}

