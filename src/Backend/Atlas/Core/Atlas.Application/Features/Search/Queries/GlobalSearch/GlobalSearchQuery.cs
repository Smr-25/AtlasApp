using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Search.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Search.Queries.GlobalSearch;

public record GlobalSearchQuery(string Query, int Limit = 5) : IRequest<GlobalSearchResultDto>;

public class GlobalSearchQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<GlobalSearchQuery, GlobalSearchResultDto>
{
    public async Task<GlobalSearchResultDto> Handle(GlobalSearchQuery request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var q = request.Query.Trim().ToLower();
        var limit = request.Limit;

        var workspaces = await context.Workspaces
            .Where(w => (w.UserProfileId == userId) && w.Name.ToLower().Contains(q))
            .Take(limit)
            .Select(w => new SearchResultDto("workspace", w.Id, w.Name, w.Description, "📁", $"/workspaces/{w.Id}"))
            .ToListAsync(ct);

        var integrations = await context.Integrations
            .Where(i => i.UserProfileId == userId && i.Name.ToLower().Contains(q))
            .Take(limit)
            .Select(i => new SearchResultDto("integration", i.Id, i.Name, i.Provider.ToString(), "🔌", "/settings/integrations"))
            .ToListAsync(ct);

        var scripts = await context.Scripts
            .Where(s => s.UserId == userId && s.Name.ToLower().Contains(q))
            .Take(limit)
            .Select(s => new SearchResultDto("script", s.Id, s.Name, s.Command, "⚡", $"/scripts/{s.Id}"))
            .ToListAsync(ct);

        var snippets = await context.Snippets
            .Where(s => s.UserId == userId && s.Title.ToLower().Contains(q))
            .Take(limit)
            .Select(s => new SearchResultDto("snippet", s.Id, s.Title, s.Language, "📝", $"/snippets/{s.Id}"))
            .ToListAsync(ct);

        var projects = await context.ProjectProfiles
            .Where(p => p.UserId == userId && p.Name.ToLower().Contains(q))
            .Take(limit)
            .Select(p => new SearchResultDto("project", p.Id, p.Name, p.RootPath, "📦", $"/projects/{p.Id}"))
            .ToListAsync(ct);

        var teams = await context.Teams
            .Where(t => t.Members.Any(m => m.UserId == userId && !m.IsDeleted) && t.Name.ToLower().Contains(q))
            .Take(limit)
            .Select(t => new SearchResultDto("team", t.Id, t.Name, null, "👥", $"/teams/{t.Id}"))
            .ToListAsync(ct);

        var commands = GetStaticCommands()
            .Where(c => c.Title.ToLower().Contains(q) || (c.Subtitle ?? "").ToLower().Contains(q))
            .Take(limit)
            .ToList();

        return new GlobalSearchResultDto(workspaces, integrations, scripts, snippets, projects, teams, commands);
    }

    private static List<SearchResultDto> GetStaticCommands() =>
    [
        new("command", Guid.Empty, "Start Focus Session", "Pomodoro timer", "🎯", "/focus"),
        new("command", Guid.Empty, "Create Script", "New automation script", "⚡", "/scripts/new"),
        new("command", Guid.Empty, "Flush Cache", "Clear caches", "🗑️", "/scripts/flush-cache"),
        new("command", Guid.Empty, "JWT Decoder", "Decode JWT token", "🔑", "/utilities/decode-jwt"),
        new("command", Guid.Empty, "Regex Tester", "Test regular expressions", "🔍", "/utilities/test-regex"),
        new("command", Guid.Empty, "JSON Formatter", "Format JSON data", "📋", "/utilities/json-format"),
        new("command", Guid.Empty, "Port Scanner", "Check port usage", "🌐", "/utilities/check-port"),
        new("command", Guid.Empty, "Kill Process", "Kill a running process", "💀", "/utilities/kill-process"),
        new("command", Guid.Empty, "Docker Containers", "Manage containers", "🐳", "/docker"),
        new("command", Guid.Empty, "Settings", "App preferences", "⚙️", "/settings"),
        new("command", Guid.Empty, "Create Workspace", "New workspace", "📁", "/workspaces/new"),
        new("command", Guid.Empty, "Notifications", "View inbox", "🔔", "/notifications"),
    ];
}
