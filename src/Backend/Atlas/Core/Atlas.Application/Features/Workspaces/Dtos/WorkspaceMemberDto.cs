using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Workspaces.Dtos;

public record WorkspaceMemberDto(
    Guid UserId,
    string UserName,
    WorkspaceMemberRole Role,
    DateTime JoinedAt
);

