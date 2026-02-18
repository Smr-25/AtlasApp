import React, { useState } from 'react';
import { postJson } from '@/lib/api';

type Integration = {
  id: string;
  name: string;
  enabled: boolean;
  logoUrl?: string | null;
};

type Props = {
  workspaceId: string;
  integration: Integration;
  onToggled?: (integrationId: string, enabled: boolean) => void;
};

const IntegrationToggle: React.FC<Props> = ({ workspaceId, integration, onToggled }) => {
  const [enabled, setEnabled] = useState<boolean>(!!integration.enabled);
  const [loading, setLoading] = useState(false);

  const toggle = async () => {
    setLoading(true);
    try {
      await postJson(`/api/workspaces/${workspaceId}/integrations/toggle`, {
        integrationId: integration.id,
        enable: !enabled,
      });
      setEnabled(!enabled);
      onToggled?.(integration.id, !enabled);
    } catch (err: any) {
      // show a simple alert for now
      alert(err?.message ?? 'Failed to toggle integration');
    } finally {
      setLoading(false);
    }
  };

  // simple logo placeholder
  const Logo = () => (
    <div className="w-6 h-6 rounded-sm bg-muted flex items-center justify-center text-xs font-semibold text-muted-foreground">
      {integration.name?.charAt(0) ?? '?'}
    </div>
  );

  return (
    <div className="flex items-center justify-between gap-3 p-2 rounded-md border border-border bg-secondary">
      <div className="flex items-center gap-3">
        {integration.logoUrl ? (
          <img src={integration.logoUrl} alt={integration.name} className="w-6 h-6 rounded-sm object-contain" />
        ) : (
          <Logo />
        )}
        <div className="text-sm">{integration.name}</div>
      </div>

      <div>
        <button
          type="button"
          onClick={toggle}
          disabled={loading}
          className={`px-3 py-1 rounded-full text-sm transition ${enabled ? 'bg-primary text-primary-foreground' : 'bg-muted text-muted-foreground'}`}
        >
          {loading ? '...' : enabled ? 'Enabled' : 'Disabled'}
        </button>
      </div>
    </div>
  );
};

export default IntegrationToggle;

