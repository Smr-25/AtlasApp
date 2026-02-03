using System.Text;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Atlas.Application.Features.Workspaces.Commands.ChatWithWorkspace;

public class ChatWithWorkspaceCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IEncryptionService encryptionService,
    IEnumerable<IIntegrationAdapter> adapters,
    IAiService aiService)
    : IRequestHandler<ChatWithWorkspaceCommand, string>
{
    public async Task<string> Handle(ChatWithWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var workspaceIntegrations = await applicationDbContext.WorkspaceIntegrations
            .Include(wi => wi.Integration)
            .Include(wi => wi.Workspace).ThenInclude(w => w.Persona)
            .Where(wi => wi.WorkspaceId == request.WorkspaceId && wi.Workspace.Persona.UserId.Equals(userId))
            .ToListAsync(cancellationToken);

        if (workspaceIntegrations.Count == 0)
        {
            return await aiService.GenerateResponseAsync(
                "You are Atlas AI, an intelligent developer companion. The user has no connected tools in their workspace. Please inform them that they need to connect tools to get insights.",
                request.Message,
                cancellationToken);
        }

        var contextBuilder = new StringBuilder();
        contextBuilder.AppendLine("Here is the current status of the user's tools:");

        foreach (var link in workspaceIntegrations)
        {
            if (link.Integration.EncryptedAccessToken == null) continue;
            var token = encryptionService.Decrypt(link.Integration.EncryptedAccessToken);

            var adapter = adapters.FirstOrDefault(a => a.Provider == link.Integration.Provider);

            if (adapter == null || string.IsNullOrEmpty(token)) continue;
            var repoName = "Unknown Repo";
            if (!string.IsNullOrEmpty(link.Config))
            {
                var json = JObject.Parse(link.Config);
                repoName = json["Name"]?.ToString() ?? "Unknown";
            }

            var resources = await adapter.GetResourcesAsync(token, cancellationToken);
            var targetRepo = resources.FirstOrDefault(r => r.Name == repoName);

            if (targetRepo == null) continue;
            contextBuilder.AppendLine($"- Tool: {link.Integration.Provider}");
            contextBuilder.AppendLine($"  Repo: {targetRepo.Name}");
            contextBuilder.AppendLine($"  Details: {targetRepo.Description}"); 
            contextBuilder.AppendLine($"  URL: {targetRepo.Url}");
        }

        const string systemPrompt = @"You are 'Atlas AI', an intelligent developer companion. 
                             Use the provided tool context (GitHub stats, etc.) to answer the user's questions accurately.
                             If the context answers the question, cite it. If not, give general advice.";

        var finalUserMessage = $"{contextBuilder}\n\nUser Question: {request.Message}";

        return await aiService.GenerateResponseAsync(systemPrompt, finalUserMessage, cancellationToken);
    }
}