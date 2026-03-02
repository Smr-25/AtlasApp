using Atlas.Application.Common.Interfaces;

namespace Atlas.Application.Common.Extensions;

public static class CurrentUserServiceExtensions
{
    extension(ICurrentUserService currentUserService)
    {
        public Guid GetRequiredUserId()
        {
            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid.");

            return parsedUserId;
        }

        public Guid? GetUserIdOrDefault()
        {
            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
                return null;

            return parsedUserId;
        }
        
        public Guid GetRequiredWorkspaceId()
        {
            var workspaceId = currentUserService.WorkspaceId;
            if (!workspaceId.HasValue || workspaceId.Value == Guid.Empty)
                throw new InvalidOperationException("X-Workspace-Id header is required. Please select a workspace.");
            return workspaceId.Value;
        }
        
        public Guid? GetWorkspaceIdOrDefault()
        {
            return currentUserService.WorkspaceId;
        }
    }
}
