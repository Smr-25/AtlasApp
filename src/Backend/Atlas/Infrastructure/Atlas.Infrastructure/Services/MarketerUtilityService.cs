using System.Text;
using System.Text.RegularExpressions;
using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class MarketerUtilityService(IAiService aiService) : IMarketerUtilityService
{
    public SeoMetaCheckResult CheckSeoMeta(string title, string description, string url)
    {
        var titleOk = title.Length is >= 30 and <= 60;
        var descOk = description.Length is >= 120 and <= 160;
        var preview = $"{title}\n{url}\n{description}";
        return new SeoMetaCheckResult(title, title.Length, titleOk, description, description.Length, descOk, preview);
    }

    public async Task<string> GenerateCopywritingAsync(string productName, string tone, CancellationToken ct)
    {
        return await aiService.GenerateResponseAsync(
            $"You are a marketing copywriter. Write 3 short catchy taglines for the product in a {tone} tone. Return only the taglines, numbered.",
            productName, ct);
    }

    public Task<byte[]> CropSocialImageAsync(Stream imageStream, string format, CancellationToken ct)
    {
        return Task.FromResult(Array.Empty<byte>());
    }

    public string ConvertMarkdownToHtml(string markdown)
    {
        var html = markdown;
        html = Regex.Replace(html, @"^### (.+)$", "<h3>$1</h3>", RegexOptions.Multiline);
        html = Regex.Replace(html, @"^## (.+)$", "<h2>$1</h2>", RegexOptions.Multiline);
        html = Regex.Replace(html, @"^# (.+)$", "<h1>$1</h1>", RegexOptions.Multiline);
        html = Regex.Replace(html, @"\*\*(.+?)\*\*", "<strong>$1</strong>");
        html = Regex.Replace(html, @"\*(.+?)\*", "<em>$1</em>");
        html = Regex.Replace(html, @"\[(.+?)\]\((.+?)\)", "<a href=\"$2\">$1</a>");
        html = Regex.Replace(html, @"^- (.+)$", "<li>$1</li>", RegexOptions.Multiline);
        html = Regex.Replace(html, @"(<li>.+</li>\n?)+", "<ul>$0</ul>");
        html = Regex.Replace(html, @"\n\n", "</p><p>");
        html = $"<p>{html}</p>";
        return html;
    }

    public KeywordDensityResult AnalyzeKeywordDensity(string content, string keyword)
    {
        var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var count = Regex.Matches(content, Regex.Escape(keyword), RegexOptions.IgnoreCase).Count;
        var density = words.Length > 0 ? (double)count / words.Length * 100 : 0;
        var recommendation = density switch
        {
            > 3 => "Too high. Reduce keyword usage to avoid spam.",
            < 1 => "Too low. Consider adding more keyword mentions.",
            _ => "Good keyword density."
        };
        return new KeywordDensityResult(keyword, count, Math.Round(density, 2), recommendation);
    }

    public ReadabilityResult CalculateReadability(string text)
    {
        var sentences = Regex.Split(text, @"[.!?]+").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var syllables = words.Sum(w => CountSyllables(w));

        var avgSentenceLength = sentences.Count > 0 ? (double)words.Length / sentences.Count : 0;
        var avgWordLength = words.Length > 0 ? words.Average(w => w.Length) : 0;
        var flesch = 206.835 - 1.015 * avgSentenceLength - 84.6 * (syllables / (double)Math.Max(words.Length, 1));
        var level = flesch switch
        {
            >= 90 => "Very Easy",
            >= 70 => "Easy",
            >= 50 => "Moderate",
            >= 30 => "Difficult",
            _ => "Very Difficult"
        };
        return new ReadabilityResult(Math.Round(flesch, 2), level, Math.Round(avgSentenceLength, 2), Math.Round(avgWordLength, 2));
    }

    public List<EmojiResult> SearchEmojis(string query)
    {
        var emojis = new Dictionary<string, (string emoji, string category)>
        {
            ["smile"] = ("😊", "Smileys"),
            ["heart"] = ("❤️", "Smileys"),
            ["fire"] = ("🔥", "Nature"),
            ["rocket"] = ("🚀", "Travel"),
            ["star"] = ("⭐", "Nature"),
            ["money"] = ("💰", "Objects"),
            ["chart"] = ("📈", "Objects"),
            ["target"] = ("🎯", "Activities"),
            ["trophy"] = ("🏆", "Activities"),
            ["check"] = ("✅", "Symbols"),
            ["warning"] = ("⚠️", "Symbols"),
            ["lock"] = ("🔒", "Objects"),
            ["key"] = ("🔑", "Objects"),
            ["email"] = ("📧", "Objects"),
            ["link"] = ("🔗", "Objects"),
            ["search"] = ("🔍", "Objects"),
            ["bulb"] = ("💡", "Objects"),
            ["globe"] = ("🌍", "Nature"),
            ["megaphone"] = ("📣", "Objects"),
            ["thumbsup"] = ("👍", "People")
        };

        return emojis
            .Where(e => e.Key.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(e => new EmojiResult(e.Value.emoji, e.Key, e.Value.category))
            .ToList();
    }

    private static int CountSyllables(string word)
    {
        word = word.ToLower().Trim();
        if (word.Length <= 3) return 1;
        var count = Regex.Matches(word, "[aeiouy]+").Count;
        if (word.EndsWith("e") && !word.EndsWith("le")) count--;
        return Math.Max(count, 1);
    }
}

