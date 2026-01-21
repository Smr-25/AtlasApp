namespace Atlas.Application.Dtos.Users;

public record UserLoginResponseDto
{
    public string Token { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}