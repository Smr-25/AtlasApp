namespace Atlas.Application.Common.Interfaces;

public interface ISocialListeningAdapter
{
    Task<List<SocialMention>> SearchMentionsAsync(string brand, string platform, CancellationToken ct);
    Task<string> ReplyToMentionAsync(string mentionId, string reply, CancellationToken ct);
}

public record SocialMention(string Id, string Platform, string Author, string Content, string Sentiment, DateTime PostedAt);

