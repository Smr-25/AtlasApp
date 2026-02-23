using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.GlobalShortcuts.Dtos;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.GlobalShortcuts.Commands.ProcessAiContext;

public class ProcessAiContextCommandHandler(
    IAiService aiService,
    ICurrentUserService currentUserService,
    IApplicationDbContext dbContext)
    : IRequestHandler<ProcessAiContextCommand, AiContextResultDto>
{
    public async Task<AiContextResultDto> Handle(ProcessAiContextCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var profile = await dbContext.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        var role = profile?.Profession.ToString() ?? "Developer";

        var systemPrompt = BuildSystemPrompt(request.Action, role);

        var result = await aiService.GenerateResponseAsync(
            systemPrompt,
            request.SelectedContent,
            cancellationToken);

        return new AiContextResultDto(
            request.Action.ToString(),
            request.SelectedContent,
            result,
            role);
    }

    private static string BuildSystemPrompt(AiContextAction action, string role) => action switch
    {
        AiContextAction.Summarize => $"You are an assistant for a {role}. Summarize the following content concisely in 2-3 sentences.",
        AiContextAction.FixGrammar => "You are a grammar expert. Fix all grammar and spelling mistakes. Return only the corrected text.",
        AiContextAction.ExplainCode => $"You are a senior {role}. Explain this code simply. What it does, why, and any issues.",
        AiContextAction.Translate => "Translate the following text to English. If already English, translate to Azerbaijani. Return only the translation.",
        AiContextAction.Refactor => "You are a code quality expert. Refactor this code for better readability and performance. Return only the code.",
        AiContextAction.GenerateTests => "You are a testing expert. Generate unit tests for this code. Use xUnit conventions.",
        AiContextAction.SimplifyText => "Simplify this text so a 12-year-old can understand it. Keep the meaning.",
        _ => "Process the following content intelligently based on context."
    };
}

