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
    }
}
