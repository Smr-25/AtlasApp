import React, { useState } from 'react';
import { postJson } from '@/lib/api';

type Props = {
  open: boolean;
  onClose: () => void;
  onCreated?: (id: string) => void;
};

const CreateWorkspaceModal: React.FC<Props> = ({ open, onClose, onCreated }) => {
  const [name, setName] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  if (!open) return null;

  const create = async () => {
    setLoading(true);
    setError(null);
    try {
      const id = await postJson<string>('/api/workspaces', { name });
      onCreated?.(id);
      onClose();
    } catch (err: any) {
      setError(err?.message ?? 'Failed to create workspace');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="w-full max-w-md p-6 rounded-2xl bg-background">
        <h3 className="text-lg font-medium mb-4">Create workspace</h3>
        <div className="space-y-3">
          <input value={name} onChange={e => setName(e.target.value)} placeholder="Workspace name" className="w-full px-3 py-2 rounded-md bg-secondary border border-border" />
          {error && <div className="text-sm text-destructive">{error}</div>}
          <div className="flex justify-end gap-3">
            <button onClick={onClose} className="px-4 py-2 rounded-lg bg-secondary">Cancel</button>
            <button onClick={create} disabled={!name || loading} className={`px-4 py-2 rounded-lg ${!name ? 'bg-muted text-muted-foreground' : 'bg-primary text-primary-foreground'}`}>
              {loading ? 'Creating...' : 'Create'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CreateWorkspaceModal;

