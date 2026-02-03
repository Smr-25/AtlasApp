namespace Atlas.Application.Settings;

public class AiSettings
{
    public const string SectionName = "ThirdPartyServices:AiSettings";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-5.2";
    public string BaseUrl { get; set; } = "https://api.openai.com/v1/";
    public double Temperature { get; set; } = 0.7;
}