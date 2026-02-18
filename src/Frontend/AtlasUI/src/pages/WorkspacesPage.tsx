import React, { useEffect, useState } from 'react';
import AtlasLogo from '@/components/AtlasLogo';
import WorkspaceCard from '@/components/workspace/WorkspaceCard';
import CreateWorkspaceModal from '@/components/workspace/CreateWorkspaceModal';
import { apiFetch } from '@/lib/api';

type WorkspaceDto = {
  id: string;
  name: string;
  createdAt?: string;
  isDefault: boolean;
  integrations?: Array<{ id: string; name: string; enabled: boolean; logoUrl?: string | null }>;
};

export default function WorkspacesPage(): JSX.Element {
  const [workspaces, setWorkspaces] = useState<WorkspaceDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await apiFetch('/api/workspaces', { method: 'GET' });
      const text = await res.text();
      let json: any;
      try { json = text ? JSON.parse(text) : null; } catch (e) { json = null; }
      const data = (json && (json.success !== undefined || json.isSuccess !== undefined)) ? json.data : json;
      setWorkspaces(data ?? []);
    } catch (err: any) {
      setError(err?.message ?? 'Failed to load workspaces');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const handleCreated = () => { load(); };
  const handleDelete = async (id: string) => {
    if (!confirm('Delete workspace?')) return;
    try {
      await apiFetch(`/api/workspaces/${id}`, { method: 'DELETE' });
      load();
    } catch (err: any) { alert(err?.message ?? 'Delete failed'); }
  };

  const handleSetDefault = async (id: string) => {
    try {
      await apiFetch(`/api/workspaces/${id}/set-default`, { method: 'PATCH' });
      load();
    } catch (err: any) { alert(err?.message ?? 'Set default failed'); }
  };

  const handleIntegrationToggled = (integrationId: string, enabled: boolean) => {
    // optimistic UI update
    setWorkspaces(ws => ws.map(w => ({ ...w, integrations: w.integrations?.map(i => i.id === integrationId ? { ...i, enabled } : i) } as WorkspaceDto)));
  };

  return (
    <div className="min-h-screen p-8 bg-background">
      <div className="max-w-4xl mx-auto space-y-6">
        <div className="flex items-center justify-between">
          <AtlasLogo />
          <div>
            <button onClick={() => setModalOpen(true)} className="px-4 py-2 rounded-lg bg-primary text-primary-foreground">Create workspace</button>
          </div>
        </div>

        {error && <div className="text-sm text-destructive">{error}</div>}

        {loading ? <div>Loading...</div> : (
          <div className="grid grid-cols-1 gap-4">
            {workspaces.length ? workspaces.map(w => (
              <WorkspaceCard key={w.id} workspace={w} onSetDefault={handleSetDefault} onDelete={handleDelete} onIntegrationToggled={handleIntegrationToggled} />
            )) : (
              <div className="text-sm text-muted-foreground">No workspaces yet.</div>
            )}
          </div>
        )}

      </div>

      <CreateWorkspaceModal open={modalOpen} onClose={() => setModalOpen(false)} onCreated={handleCreated} />
    </div>
  );
}
