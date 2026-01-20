namespace Atlas.Application.Models;

public class SmsSettings
{
    public string AccountsId { get; set; } = null!;
    public string AuthToken { get; set; } = null!;
    public string FromNumber { get; set; } = null!;
}