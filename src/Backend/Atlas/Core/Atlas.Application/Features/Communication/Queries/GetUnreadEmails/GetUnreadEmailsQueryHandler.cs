using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Communication.Dtos;
using MediatR;

namespace Atlas.Application.Features.Communication.Queries.GetUnreadEmails;

public class GetUnreadEmailsQueryHandler(IGmailService gmailService)
    : IRequestHandler<GetUnreadEmailsQuery, List<EmailDto>>
{
    public async Task<List<EmailDto>> Handle(GetUnreadEmailsQuery request, CancellationToken cancellationToken)
    {
        return await gmailService.GetUnreadEmailsAsync(cancellationToken);
    }
}