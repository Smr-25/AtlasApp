using Atlas.Application.Common.Interfaces;
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
            services.AddHttpContextAccessor();
            services.Configure<EmailSettings>(configuration.GetSection("ThirdPartyServices:EmailSettings"));
            services.Configure<SmsSettings>(configuration.GetSection("ThirdPartyServices:SmsSettings"));
            services.Configure<TelegramSettings>(configuration.GetSection("ThirdPartyServices:TelegramSettings"));
            services.Configure<ExternalAuthSettings>(configuration.GetSection("ExternalAuthSettings"));
            
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<ITelegramService, TelegramService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>(); 
            services.AddScoped<IPhoneVerificationService, PhoneVerificationService>();
        }
    }
}