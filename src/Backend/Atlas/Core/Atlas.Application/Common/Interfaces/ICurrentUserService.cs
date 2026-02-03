namespace Atlas.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    string? Language { get; }      
    int TimezoneOffsetInMinutes { get; }
}