using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.GlobalShortcuts.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.GlobalShortcuts.Queries.SearchCommandPalette;

public class SearchCommandPaletteQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<SearchCommandPaletteQuery, CommandPaletteResultDto>
{
    private static readonly List<CommandPaletteItemDto> StaticCommands =
    [
        new("cmd-focus", "Toggle Focus Mode", "Scripts", "🎯", "Script", "ToggleFocusMode"),
        new("cmd-snippet", "New Snippet", "Scripts", "📝", "Script", "QuickSnippet"),
        new("cmd-terminal", "Toggle Terminal", "System", "💻", "System", "ToggleTerminal"),
        new("cmd-docker-up", "Spin Up Docker", "Scripts", "🐳", "Script", "SpinEnvironment"),
        new("cmd-format", "Format & Lint", "Scripts", "✨", "Script", "FormatAndLint"),
        new("cmd-kill-ports", "Kill Port", "Utilities", "🔌", "Utility", "PortKiller"),
        new("cmd-jwt", "Decode JWT", "Utilities", "🔑", "Utility", "DecodeJwt"),
        new("cmd-base64", "Base64 Convert", "Utilities", "🔄", "Utility", "Base64"),
        new("cmd-regex", "Test Regex", "Utilities", "🔍", "Utility", "TestRegex"),
        new("cmd-hash", "Generate Hash", "Security", "🛡️", "Utility", "GenerateHash"),
        new("cmd-ssl", "Check SSL", "Security", "🔒", "Utility", "CheckSsl"),
        new("cmd-scan-ports", "Scan Ports", "Security", "📡", "Utility", "ScanPorts"),
        new("cmd-seo", "SEO Check", "Marketing", "📊", "Utility", "SeoCheck"),
        new("cmd-readability", "Readability Score", "Marketing", "📖", "Utility", "Readability"),
        new("cmd-contrast", "Contrast Checker", "Design", "🎨", "Utility", "ContrastCheck"),
        new("cmd-svg", "Optimize SVG", "Design", "🖼️", "Utility", "SvgOptimize"),
        new("cmd-bg-remove", "Remove Background", "Design", "✂️", "Utility", "BgRemove")
    ];

    public async Task<CommandPaletteResultDto> Handle(SearchCommandPaletteQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        var term = request.SearchTerm?.Trim().ToLowerInvariant() ?? "";

        var items = new List<CommandPaletteItemDto>();

        var matchedStatic = string.IsNullOrEmpty(term)
            ? StaticCommands
            : StaticCommands.Where(c =>
                c.Label.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                c.Category.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
        items.AddRange(matchedStatic);

        var scripts = await dbContext.Scripts
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .Where(s => string.IsNullOrEmpty(term) || s.Name.ToLower().Contains(term))
            .Take(5)
            .Select(s => new CommandPaletteItemDto(
                s.Id.ToString(), s.Name, "My Scripts", "⚡", "CustomScript", s.Id.ToString()))
            .ToListAsync(cancellationToken);
        items.AddRange(scripts);

        var snippets = await dbContext.Snippets
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .Where(s => string.IsNullOrEmpty(term) || s.Title.ToLower().Contains(term))
            .Take(5)
            .Select(s => new CommandPaletteItemDto(
                s.Id.ToString(), s.Title, "Snippets", "📋", "Snippet", s.Id.ToString()))
            .ToListAsync(cancellationToken);
        items.AddRange(snippets);

        return new CommandPaletteResultDto(items, items.Count);
    }
}

