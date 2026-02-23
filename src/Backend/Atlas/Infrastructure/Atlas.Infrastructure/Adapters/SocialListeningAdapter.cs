using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class SocialListeningAdapter(IHttpClientFactory httpClientFactory) : ISocialListeningAdapter
{
    public Task<List<SocialMention>> SearchMentionsAsync(string brand, string platform, CancellationToken ct)
        => Task.FromResult(new List<SocialMention>());

    public Task<string> ReplyToMentionAsync(string mentionId, string reply, CancellationToken ct)
        => Task.FromResult($"Reply sent to mention {mentionId}.");
}

