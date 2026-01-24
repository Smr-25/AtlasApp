namespace Atlas.Application.Dtos.Users.Auth;

public record UserLoginResponseDto
{
    public string Token { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}