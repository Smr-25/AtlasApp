using Atlas.Application.Features.Communication.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IGmailService
{
    Task<List<EmailDto>> GetUnreadEmailsAsync(CancellationToken cancellationToken = default);
}