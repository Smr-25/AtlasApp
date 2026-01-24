using Atlas.Application.Services.Interfaces;
using Atlas.Application.Settings;
using Atlas.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Infrastructure;

public static class ServiceRegistration
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructureServices(IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("ThirdPartyServices:EmailSettings"));
            services.Configure<SmsSettings>(configuration.GetSection("ThirdPartyServices:SmsSettings"));
            services.Configure<TelegramSettings>(configuration.GetSection("ThirdPartyServices:TelegramSettings"));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<ITelegramService, TelegramService>();
        }
    }
}