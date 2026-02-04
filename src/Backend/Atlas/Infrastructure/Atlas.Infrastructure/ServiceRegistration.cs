using Atlas.Application.Common.Interfaces;
using Atlas.Application.Services.Interfaces;
using Atlas.Application.Settings;
using Atlas.Infrastructure.Adapters;
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
            services.AddHttpClient("GitHub");
            services.AddHttpClient("OpenAI", client =>
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration.GetSection("ThirdPartyServices:AiSettings:ApiKey").Value}");
            });
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
            services.Configure<SmsSettings>(configuration.GetSection(SmsSettings.SectionName));
            services.Configure<TelegramSettings>(configuration.GetSection(TelegramSettings.SectionName));
            services.Configure<ExternalAuthSettings>(configuration.GetSection(ExternalAuthSettings.SectionName));
            services.Configure<AiSettings>(configuration.GetSection(AiSettings.SectionName));
            
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<ITelegramService, TelegramService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>(); 
            services.AddScoped<IPhoneVerificationService, PhoneVerificationService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IIntegrationAdapter, GitHubAdapter>();
            services.AddScoped<IAiService, OpenAiService>();
            services.AddScoped<IGreetingService, GreetingService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddSingleton<IDockerService, DockerService>();
            services.AddTransient<ISystemMonitorService, SystemMonitorService>();
            services.AddTransient<IScriptRunnerService, ScriptRunnerService>();
            services.AddTransient<INotionService, NotionService>();
            services.AddTransient<IGmailService, GmailService>();
        }
    }
}