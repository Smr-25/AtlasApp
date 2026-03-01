import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import {
  FolderOpen,
  Plus,
  Star,
  Trash2,
  MoreHorizontal,
  Plug,
  Check,
  Loader2,
} from "lucide-react";
import { WorkspaceDto } from "@/services/api";

interface WorkspacesPanelProps {
  workspaces: WorkspaceDto[];
  activeWorkspace: WorkspaceDto | null;
  onSwitchWorkspace: (ws: WorkspaceDto) => void;
  onCreateWorkspace: () => void;
  onDeleteWorkspace: (id: string) => Promise<void>;
  onSetDefault: (id: string) => Promise<void>;
}

const WorkspacesPanel = ({
  workspaces,
  activeWorkspace,
  onSwitchWorkspace,
  onCreateWorkspace,
  onDeleteWorkspace,
  onSetDefault,
}: WorkspacesPanelProps) => {
  const [menuOpen, setMenuOpen] = useState<string | null>(null);
  const [deleting, setDeleting] = useState<string | null>(null);

  const handleDelete = async (id: string) => {
    setDeleting(id);
    await onDeleteWorkspace(id);
    setDeleting(null);
    setMenuOpen(null);
  };

  return (
    <div className="space-y-5">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-bold text-foreground">Workspaces</h2>
          <p className="text-sm text-muted-foreground">Manage your project workspaces</p>
        </div>
        <motion.button
          whileHover={{ scale: 1.02 }}
          whileTap={{ scale: 0.98 }}
          onClick={onCreateWorkspace}
          className="flex items-center gap-2 px-4 h-9 rounded-lg bg-primary text-primary-foreground text-sm font-medium shadow-md shadow-primary/20"
        >
          <Plus className="w-4 h-4" />
          New Workspace
        </motion.button>
      </div>

      {/* Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
        {workspaces.map((ws, i) => (
          <motion.div
            key={ws.id}
            initial={{ opacity: 0, y: 15 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: i * 0.05 }}
            whileHover={{ y: -3, boxShadow: "0 12px 40px -15px hsl(var(--primary) / 0.12)" }}
            onClick={() => onSwitchWorkspace(ws)}
            className={`relative bg-card rounded-xl border p-5 cursor-pointer transition-all ${
              ws.id === activeWorkspace?.id ? "border-primary/40 shadow-sm shadow-primary/5" : "border-border hover:border-primary/20"
            }`}
          >
            {/* Menu */}
            <div className="absolute top-3 right-3">
              <button
                onClick={(e) => { e.stopPropagation(); setMenuOpen(menuOpen === ws.id ? null : ws.id); }}
                className="w-7 h-7 rounded-md flex items-center justify-center text-muted-foreground hover:bg-muted transition-colors"
              >
                <MoreHorizontal className="w-4 h-4" />
              </button>
              <AnimatePresence>
                {menuOpen === ws.id && (
                  <motion.div
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    exit={{ opacity: 0, scale: 0.9 }}
                    className="absolute right-0 top-8 w-40 bg-card border border-border rounded-lg shadow-xl z-20 overflow-hidden"
                    onClick={(e) => e.stopPropagation()}
                  >
                    {!ws.isDefault && (
                      <button
                        onClick={() => { onSetDefault(ws.id); setMenuOpen(null); }}
                        className="w-full flex items-center gap-2 px-3 py-2 text-xs text-foreground hover:bg-muted transition-colors"
                      >
                        <Star className="w-3.5 h-3.5" />
                        Set as default
                      </button>
                    )}
                    <button
                      onClick={() => handleDelete(ws.id)}
                      disabled={deleting === ws.id}
                      className="w-full flex items-center gap-2 px-3 py-2 text-xs text-red-500 hover:bg-red-500/10 transition-colors"
                    >
                      {deleting === ws.id ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Trash2 className="w-3.5 h-3.5" />}
                      Delete
                    </button>
                  </motion.div>
                )}
              </AnimatePresence>
            </div>

            {/* Icon & Name */}
            <div className="flex items-start gap-3 mb-4">
              <div className={`w-10 h-10 rounded-xl flex items-center justify-center shrink-0 ${
                ws.isDefault ? "bg-primary/10" : "bg-muted/60"
              }`}>
                {ws.isDefault ? <Star className="w-5 h-5 text-primary fill-primary/20" /> : <FolderOpen className="w-5 h-5 text-muted-foreground" />}
              </div>
              <div className="min-w-0">
                <h3 className="text-sm font-semibold text-foreground truncate">{ws.name}</h3>
                <p className="text-[11px] text-muted-foreground truncate">{ws.description || "No description"}</p>
              </div>
            </div>

            {/* Integrations */}
            <div className="flex items-center gap-2 mb-3">
              <Plug className="w-3.5 h-3.5 text-muted-foreground" />
              <span className="text-[11px] text-muted-foreground">
                {ws.activeIntegrations?.length || 0} active integration{(ws.activeIntegrations?.length || 0) !== 1 ? "s" : ""}
              </span>
            </div>

            {/* Integration avatars */}
            {ws.activeIntegrations && ws.activeIntegrations.length > 0 && (
              <div className="flex -space-x-1.5">
                {ws.activeIntegrations.slice(0, 5).map((int) => (
                  <div
                    key={int.integrationId}
                    className="w-6 h-6 rounded-full bg-muted border-2 border-card flex items-center justify-center"
                    title={int.integrationName}
                  >
                    <span className="text-[8px] font-bold text-muted-foreground">{int.provider.slice(0, 2)}</span>
                  </div>
                ))}
                {ws.activeIntegrations.length > 5 && (
                  <div className="w-6 h-6 rounded-full bg-muted border-2 border-card flex items-center justify-center">
                    <span className="text-[8px] font-bold text-muted-foreground">+{ws.activeIntegrations.length - 5}</span>
                  </div>
                )}
              </div>
            )}

            {/* Badges */}
            <div className="flex items-center gap-2 mt-3">
              {ws.isDefault && (
                <span className="text-[9px] font-medium text-primary bg-primary/10 px-2 py-0.5 rounded-full">Default</span>
              )}
              {ws.id === activeWorkspace?.id && (
                <span className="text-[9px] font-medium text-emerald-600 bg-emerald-500/10 px-2 py-0.5 rounded-full flex items-center gap-1">
                  <Check className="w-2.5 h-2.5" />Active
                </span>
              )}
            </div>
          </motion.div>
        ))}

        {/* Create New Card */}
        <motion.button
          initial={{ opacity: 0, y: 15 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: workspaces.length * 0.05 }}
          whileHover={{ y: -3, borderColor: "hsl(var(--primary) / 0.3)" }}
          onClick={onCreateWorkspace}
          className="flex flex-col items-center justify-center gap-3 p-8 rounded-xl border-2 border-dashed border-border bg-card/50 text-muted-foreground hover:text-primary transition-all min-h-[180px]"
        >
          <div className="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center">
            <Plus className="w-6 h-6 text-primary" />
          </div>
          <div className="text-center">
            <p className="text-sm font-medium">Create Workspace</p>
            <p className="text-[11px] text-muted-foreground">Add a new project space</p>
          </div>
        </motion.button>
      </div>
    </div>
  );
};

export default WorkspacesPanel;


