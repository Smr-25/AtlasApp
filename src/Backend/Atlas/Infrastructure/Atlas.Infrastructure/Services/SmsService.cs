using Atlas.Application.Models;
using Atlas.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Atlas.Infrastructure.Services;

public class SmsService(IOptions<SmsSettings> options) : ISmsService
{
    private readonly SmsSettings _settings = options.Value;

    public async Task SendSmsAsync(string to, string code)
    {
        await MessageResource.CreateAsync(
            to: new PhoneNumber(to),
            from: new PhoneNumber(_settings.FromNumber),
            body: code
        );
    }

    public async Task SendVerificationSmsAsync(string to, string code)
    {
        await SendSmsAsync(to,"Your verification code is: " + code);
    }

    public async Task SendPasswordResetSmsAsync(string to, string resetCode)
    {
        await SendSmsAsync(to, "Your password reset code is: " + resetCode);
    }
}