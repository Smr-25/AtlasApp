using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.GlobalShortcuts.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.GlobalShortcuts.Commands.QuickShare;

public class QuickShareCommandHandler(
    ICurrentUserService currentUserService,
    IEmailService emailService,
    ITelegramService telegramService)
    : IRequestHandler<QuickShareCommand, QuickShareResultDto>
{
    public async Task<QuickShareResultDto> Handle(QuickShareCommand request, CancellationToken cancellationToken)
    {
        currentUserService.GetRequiredUserId();

        switch (request.Channel)
        {
            case ShareChannel.Gmail:
                if (string.IsNullOrWhiteSpace(request.RecipientEmail))
                    return new QuickShareResultDto("Gmail", false, null);
                await emailService.SendEmailAsync(
                    request.RecipientEmail,
                    "Shared from Atlas",
                    request.Content);
                return new QuickShareResultDto("Gmail", true, null);

            case ShareChannel.Telegram:
                await telegramService.SendMessageAsync("default", request.Content);
                return new QuickShareResultDto("Telegram", true, null);

            case ShareChannel.Slack:
                return new QuickShareResultDto("Slack", true, request.SlackChannel);

            default:
                return new QuickShareResultDto(request.Channel.ToString(), false, null);
        }
    }
}

