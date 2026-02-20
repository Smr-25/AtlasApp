namespace Atlas.Application.Settings;

public class StripeSettings
{
    public const string SectionName = "StripeSettings";
    
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string FreePriceId { get; set; } = string.Empty;
    public string ProPriceId { get; set; } = string.Empty;
    public string TeamPriceId { get; set; } = string.Empty;
}

