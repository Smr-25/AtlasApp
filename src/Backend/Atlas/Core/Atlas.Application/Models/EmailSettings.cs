namespace Atlas.Application.Models;

public class EmailSettings
{
    public string SmtpServer { get; set; } = null!;
    public int Port { get; set; }
    public string SenderEmail { get; set; } = null!;
    public string SenderName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}