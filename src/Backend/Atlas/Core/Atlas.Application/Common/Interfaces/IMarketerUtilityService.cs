namespace Atlas.Application.Common.Interfaces;

public interface IMarketerUtilityService
{
    SeoMetaCheckResult CheckSeoMeta(string title, string description, string url);
    Task<string> GenerateCopywritingAsync(string productName, string tone, CancellationToken ct);
    Task<byte[]> CropSocialImageAsync(Stream imageStream, string format, CancellationToken ct);
    string ConvertMarkdownToHtml(string markdown);
    KeywordDensityResult AnalyzeKeywordDensity(string content, string keyword);
    ReadabilityResult CalculateReadability(string text);
    List<EmojiResult> SearchEmojis(string query);
}

public record SeoMetaCheckResult(string Title, int TitleLength, bool TitleOk, string Description, int DescriptionLength, bool DescriptionOk, string PreviewSnippet);
public record KeywordDensityResult(string Keyword, int Count, double Density, string Recommendation);
public record ReadabilityResult(double FleschScore, string Level, double AvgSentenceLength, double AvgWordLength);
public record EmojiResult(string Emoji, string Name, string Category);

