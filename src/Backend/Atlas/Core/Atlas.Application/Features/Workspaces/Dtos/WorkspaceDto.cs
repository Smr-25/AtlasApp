using System;
using System.Collections.Generic;
using Atlas.Application.Features.Integrations.Dtos;

namespace Atlas.Application.Features.Workspaces.Dtos;

public class WorkspaceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public string? LocalFolderPath { get; set; }
    public bool IsShared { get; set; }
    public List<WorkspaceIntegrationDto> ActiveIntegrations { get; set; } = [];
}    
