using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Helpers;
using Atlas.Application.Common.Models;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.ResendPhoneVerification;

public class ResendPhoneVerificationCommandHandler(UserManager<AppUser> userManager, ISmsService smsService,ITelegramService telegramService) : IRequestHandler<ResendPhoneVerificationCommand,ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(ResendPhoneVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
        if (user == null)
            throw new NotFoundException("User", request.PhoneNumber);

        if (user.PhoneNumberConfirmed)
            throw new AlreadyVerifiedException("Phone Number");

        await SendPhoneVerificationCodeAsync(user, request.Channel);
        return ResponseModel<bool>.Success(true);
    }

    private async Task SendPhoneVerificationCodeAsync(AppUser user, UserVerificationChannel requestPhoneVerificationChannel)
    {
        var code = VerificationCodeGenerator.Generate();
        user.PhoneVerificationCode = code;
        user.PhoneVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);

        switch (requestPhoneVerificationChannel)
        {
            case UserVerificationChannel.Sms:
                await smsService.SendVerificationSmsAsync(user.PhoneNumber!, code);
                break;
            case UserVerificationChannel.Telegram:
                if (!string.IsNullOrEmpty(user.TelegramChatId))
                    await telegramService.SendVerificationCodeAsync(user.TelegramChatId, code);
                break;
            default:
                throw new InvalidVerificationChannelException("The selected phone verification channel is invalid.");
        }
    }
}