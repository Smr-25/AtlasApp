namespace Atlas.Application.Common.Interfaces;

public interface IActivityService
{
    Task LogAsync(Guid userId, string actionType, string description, Guid? workspaceId = null, CancellationToken cancellationToken = default);
}