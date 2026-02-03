namespace Atlas.Application.Settings;

public class EmailSettings
{
    public const string SectionName = "ThirdPartyServices:EmailSettings";
    public string SmtpServer { get; set; } = null!;
    public int Port { get; set; }
    public string SenderEmail { get; set; } = null!;
    public string SenderName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool UseSsl { get; set; }
}