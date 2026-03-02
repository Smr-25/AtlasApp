using System;
using System.Collections.Generic;
using Atlas.Application.Features.Integrations.Dtos;
using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Workspaces.Dtos;

public class WorkspaceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public string? LocalFolderPath { get; set; }
    public bool IsShared { get; set; }
    public WorkspaceMemberRole MyRole { get; set; }
    public int MembersCount { get; set; }
    public List<WorkspaceIntegrationDto> ActiveIntegrations { get; set; } = [];
}    
