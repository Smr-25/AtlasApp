using Atlas.Application.Common.Interfaces;
using Atlas.Application.Settings;
using Atlas.Infrastructure.Adapters;
using Atlas.Infrastructure.Services;
using Hardware.Info;
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
            services.AddDataProtection();
            services.AddHttpClient("GitHub");
            services.AddHttpClient("Jira");
            services.AddHttpClient("OpenAI", client =>
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration.GetSection("ThirdPartyServices:AiSettings:ApiKey").Value}");
            });
            
            services.AddHttpClient("AtlasClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "Atlas-SuperApp/1.0");
            });
            
            services.AddHttpClient<IAiAdvisorService, AiAdvisorService>(); 
            
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
            services.Configure<SmsSettings>(configuration.GetSection(SmsSettings.SectionName));
            services.Configure<TelegramSettings>(configuration.GetSection(TelegramSettings.SectionName));
            services.Configure<ExternalAuthSettings>(configuration.GetSection(ExternalAuthSettings.SectionName));
            services.Configure<AiSettings>(configuration.GetSection(AiSettings.SectionName));
            services.Configure<StripeSettings>(configuration.GetSection(StripeSettings.SectionName));
            
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
            
            services.AddTransient<IGitIntegrationAdapter, GitHubAdapter>();
            services.AddTransient<IJiraAdapter, JiraAdapter>();
            services.AddSingleton<IDockerAdapter, DockerAdapter>();
            services.AddSingleton<IHardwareInfo>(_ =>
            {
                var hardwareInfo = new HardwareInfo();
                return hardwareInfo;
            });
            services.AddTransient<ISystemMonitorService, SystemMonitorService>();
            services.AddTransient<IScriptRunnerService, ScriptRunnerService>();
            services.AddTransient<INotionService, NotionService>();
            services.AddTransient<IGmailService, GmailService>();
            services.AddTransient<IMigrationBuilderService, MigrationBuilderService>();
            services.AddTransient<IImageProcessingService, ImageProcessingService>();
            services.AddTransient<ISystemToolAdapter,SystemToolAdapter>();
            services.AddTransient<INetworkToolAdapter, NetworkToolAdapter>();
            services.AddTransient<IJsonToolService, JsonToolService>();
            services.AddTransient<ISecurityToolAdapter, SecurityToolAdapter>();
            services.AddScoped<ISubscriptionGuardService, SubscriptionGuardService>();
            services.AddScoped<IStripeService, StripeService>();
            services.AddTransient<IFileSystemService, FileSystemService>();
            
            services.AddTransient<ISentryAdapter, SentryAdapter>();
            services.AddTransient<IAwsAdapter, AwsAdapter>();
            services.AddTransient<ISonarQubeAdapter, SonarQubeAdapter>();
            services.AddTransient<IPerplexityAdapter, PerplexityAdapter>();
            services.AddTransient<IFigmaAdapter, FigmaAdapter>();
            services.AddTransient<IMiroAdapter, MiroAdapter>();
            services.AddTransient<ILottieFilesAdapter, LottieFilesAdapter>();
            services.AddTransient<IDribbbleAdapter, DribbbleAdapter>();
            services.AddTransient<IZeplinAdapter, ZeplinAdapter>();
            services.AddTransient<IDevUtilityService, DevUtilityService>();
            services.AddTransient<IDesignUtilityService, DesignUtilityService>();
            services.AddTransient<IProactiveAgentService, ProactiveAgentService>();
            services.AddScoped<IInsightCalculationService, InsightCalculationService>();
            services.AddScoped<IDesignInsightCalculationService, DesignInsightCalculationService>();
            
            services.AddTransient<ISecOpsUtilityService, SecOpsUtilityService>();
            services.AddTransient<ISecOpsAgentService, SecOpsAgentService>();
            services.AddScoped<ISecOpsInsightCalculationService, SecOpsInsightCalculationService>();
            services.AddTransient<IMarketerUtilityService, MarketerUtilityService>();
            services.AddTransient<IMarketerAgentService, MarketerAgentService>();
            services.AddScoped<IMarketerInsightCalculationService, MarketerInsightCalculationService>();
            
            services.AddTransient<ILeaderScriptService, LeaderScriptService>();
            services.AddTransient<ILeaderUtilityService, LeaderUtilityService>();
            services.AddTransient<ILeaderAgentService, LeaderAgentService>();
            services.AddScoped<ILeaderInsightCalculationService, LeaderInsightCalculationService>();
            services.AddScoped<ISquadRadarService, SquadRadarService>();
            services.AddScoped<IOmniFeedService, OmniFeedService>();
            
            services.AddTransient<ICloudflareAdapter, CloudflareAdapter>();
            services.AddTransient<ISnykAdapter, SnykAdapter>();
            services.AddTransient<IAwsGuardDutyAdapter, AwsGuardDutyAdapter>();
            services.AddTransient<IVirusTotalAdapter, VirusTotalAdapter>();
            services.AddTransient<IShodanAdapter, ShodanAdapter>();
            services.AddTransient<IPagerDutyAdapter, PagerDutyAdapter>();
            services.AddTransient<IMetaAdsAdapter, MetaAdsAdapter>();
            services.AddTransient<IGoogleSearchConsoleAdapter, GoogleSearchConsoleAdapter>();
            services.AddTransient<IMailchimpAdapter, MailchimpAdapter>();
            services.AddTransient<ISocialListeningAdapter, SocialListeningAdapter>();
            services.AddTransient<IGA4Adapter, GA4Adapter>();
            services.AddTransient<IHubSpotAdapter, HubSpotAdapter>();
        }
    }
}