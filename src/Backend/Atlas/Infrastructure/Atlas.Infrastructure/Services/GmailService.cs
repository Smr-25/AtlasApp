using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Communication.Dtos;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Atlas.Infrastructure.Services;

public class GmailService : IGmailService
{
    private static readonly string[] Scopes = [Google.Apis.Gmail.v1.GmailService.Scope.GmailReadonly];
    private const string ApplicationName = "Atlas Developer App";

    public async Task<List<EmailDto>> GetUnreadEmailsAsync(CancellationToken cancellationToken = default)
    {
        UserCredential credential;

        await using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            const string credPath = "token.json"; 

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                (await GoogleClientSecrets.FromStreamAsync(stream, cancellationToken)).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));
        }

        var service = new Google.Apis.Gmail.v1.GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        var request = service.Users.Messages.List("me");
        request.Q = "is:unread";
        request.MaxResults = 10; 

        var response = await request.ExecuteAsync(cancellationToken);
        var emails = new List<EmailDto>();

        if (response.Messages == null || response.Messages.Count <= 0) return emails;
        
        foreach (var msgItem in response.Messages)
        {
            var msgDetail = await service.Users.Messages.Get("me", msgItem.Id).ExecuteAsync(cancellationToken);
            var headers = msgDetail.Payload.Headers;
            var subject = headers.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "(No Subject)";
            var from = headers.FirstOrDefault(h => h.Name == "From")?.Value ?? "Unknown";
            var date = headers.FirstOrDefault(h => h.Name == "Date")?.Value ?? "";

            if (from.Contains("<")) from = from.Split('<')[0].Trim().Trim('"');

            emails.Add(new EmailDto(
                msgItem.Id,
                from,
                subject,
                msgDetail.Snippet, 
                date
            ));
        }
        return emails;
    }
}