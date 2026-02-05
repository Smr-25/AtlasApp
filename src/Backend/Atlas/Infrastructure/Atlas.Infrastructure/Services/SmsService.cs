using Atlas.Application.Common.Interfaces;
using Atlas.Application.Settings;
using Microsoft.Extensions.Options;
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
        await SendSmsAsync(to, "Your verification code is: " + code);
    }
}