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

namespace Atlas.Application.Features.Accounts.Commands.AddPhoneNumber;

public class AddPhoneNumberCommandHandler(
    UserManager<AppUser> userManager,
    ISmsService smsService,
    ITelegramService telegramService) : IRequestHandler<AddPhoneNumberCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(AddPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            throw new NotFoundException("User", request.UserId);

        if (!string.IsNullOrEmpty(user.PhoneNumber))
            throw new BadRequestException("Phone number already exists. Use update phone number instead.");

        var existingPhone = await userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

        if (existingPhone != null)
            throw new AlreadyExistException("PhoneNumber", request.PhoneNumber);

        user.PhoneNumber = request.PhoneNumber;
        user.PreferredVerificationChannel = request.VerificationChannel;
        await userManager.UpdateAsync(user);

        await SendPhoneVerificationCodeAsync(user, request.VerificationChannel);

        return ResponseModel<bool>.Success(true);
    }

    private async Task SendPhoneVerificationCodeAsync(AppUser user,
        UserVerificationChannel requestPhoneVerificationChannel)
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