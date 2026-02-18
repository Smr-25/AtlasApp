import React from 'react';
import IntegrationToggle from './IntegrationToggle';

type IntegrationDto = { id: string; name: string; enabled: boolean; logoUrl?: string | null };

type Props = {
  workspace: {
    id: string;
    name: string;
    isDefault: boolean;
    createdAt?: string;
    integrations?: IntegrationDto[];
  };
  onSetDefault?: (id: string) => void;
  onDelete?: (id: string) => void;
  onIntegrationToggled?: (integrationId: string, enabled: boolean) => void;
};

const WorkspaceCard: React.FC<Props> = ({ workspace, onSetDefault, onDelete, onIntegrationToggled }) => {
  return (
    <div className="p-4 rounded-lg border border-border bg-secondary">
      <div className="flex items-start justify-between">
        <div>
          <div className="text-lg font-medium">{workspace.name}</div>
          <div className="text-sm text-muted-foreground">{workspace.createdAt ? new Date(workspace.createdAt).toLocaleString() : ''}</div>
        </div>
        <div className="flex items-center gap-2">
          {!workspace.isDefault && (
            <button onClick={() => onSetDefault?.(workspace.id)} className="text-sm px-3 py-1 rounded-lg bg-primary text-primary-foreground">Set default</button>
          )}
          <button onClick={() => onDelete?.(workspace.id)} className="text-sm px-3 py-1 rounded-lg bg-destructive text-destructive-foreground">Delete</button>
        </div>
      </div>

      <div className="mt-4 space-y-2">
        {workspace.integrations && workspace.integrations.length ? (
          workspace.integrations.map(i => (
            <IntegrationToggle key={i.id} workspaceId={workspace.id} integration={i} onToggled={(iid, enabled) => onIntegrationToggled?.(iid, enabled)} />
          ))
        ) : (
          <div className="text-sm text-muted-foreground">No integrations</div>
        )}
      </div>
    </div>
  );
};

export default WorkspaceCard;

