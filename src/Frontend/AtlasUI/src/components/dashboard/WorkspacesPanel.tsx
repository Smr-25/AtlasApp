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
  Link2,
  X,
  ArrowRight,
} from "lucide-react";
import { WorkspaceDto, IntegrationDto } from "@/services/api";
import { getProviderIcon } from "@/components/icons/IntegrationIcons";
import { getProviderInfo } from "@/lib/integration-providers";

interface WorkspacesPanelProps {
  workspaces: WorkspaceDto[];
  activeWorkspace: WorkspaceDto | null;
  integrations: IntegrationDto[];
  onSwitchWorkspace: (ws: WorkspaceDto) => void;
  onCreateWorkspace: () => void;
  onDeleteWorkspace: (id: string) => Promise<void>;
  onSetDefault: (id: string) => Promise<void>;
  onToggleIntegration: (workspaceId: string, integrationId: string, enable: boolean) => Promise<void>;
  onEnterWorkspace?: (ws: WorkspaceDto) => void;
}

const WorkspacesPanel = ({
  workspaces,
  activeWorkspace,
  integrations,
  onSwitchWorkspace,
  onCreateWorkspace,
  onDeleteWorkspace,
  onSetDefault,
  onToggleIntegration,
  onEnterWorkspace,
}: WorkspacesPanelProps) => {
  const [menuOpen, setMenuOpen] = useState<string | null>(null);
  const [deleting, setDeleting] = useState<string | null>(null);
  const [manageWsId, setManageWsId] = useState<string | null>(null);
  const [toggling, setToggling] = useState<string | null>(null);

  // Always get fresh workspace data from props
  const manageWs = manageWsId ? workspaces.find((w) => w.id === manageWsId) || null : null;

  const handleDelete = async (id: string) => {
    setDeleting(id);
    await onDeleteWorkspace(id);
    setDeleting(null);
    setMenuOpen(null);
  };

  const handleToggle = async (wsId: string, intId: string, enable: boolean) => {
    setToggling(intId);
    await onToggleIntegration(wsId, intId, enable);
    setToggling(null);
  };

  // Only Active integrations can be toggled to workspaces
  const activeIntegrations = integrations.filter((i) => i.status === "Active");

  return (
    <div className="space-y-5">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-bold text-foreground">Workspaces</h2>
          <p className="text-sm text-muted-foreground">Manage your project workspaces and their integrations</p>
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
            className={`relative bg-card rounded-xl border p-5 cursor-pointer transition-all ${
              ws.id === activeWorkspace?.id ? "border-primary/40 shadow-sm shadow-primary/5" : "border-border hover:border-primary/20"
            }`}
          >
            {/* Menu */}
            <div className="absolute top-3 right-3 flex items-center gap-1">
              <button
                onClick={(e) => { e.stopPropagation(); setManageWsId(ws.id); }}
                className="w-7 h-7 rounded-md flex items-center justify-center text-muted-foreground hover:bg-primary/10 hover:text-primary transition-colors"
                title="Manage integrations"
              >
                <Plug className="w-3.5 h-3.5" />
              </button>
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

            <div onClick={() => onSwitchWorkspace(ws)}>
              {/* Icon & Name */}
              <div className="flex items-start gap-3 mb-4">
                <div className={`w-10 h-10 rounded-xl flex items-center justify-center shrink-0 ${
                  ws.isDefault ? "bg-primary/10" : "bg-muted/60"
                }`}>
                  {ws.isDefault ? <Star className="w-5 h-5 text-primary fill-primary/20" /> : <FolderOpen className="w-5 h-5 text-muted-foreground" />}
                </div>
                <div className="min-w-0 pr-16">
                  <h3 className="text-sm font-semibold text-foreground truncate">{ws.name}</h3>
                  <p className="text-[11px] text-muted-foreground truncate">{ws.description || "No description"}</p>
                </div>
              </div>

              {/* Integrations count */}
              <div className="flex items-center gap-2 mb-3">
                <Plug className="w-3.5 h-3.5 text-muted-foreground" />
                <span className="text-[11px] text-muted-foreground">
                  {ws.activeIntegrations?.length || 0} integration{(ws.activeIntegrations?.length || 0) !== 1 ? "s" : ""} linked
                </span>
              </div>

              {/* Integration avatars with real SVG icons */}
              {ws.activeIntegrations && ws.activeIntegrations.length > 0 && (
                <div className="flex items-center gap-1.5 flex-wrap mb-3">
                  {ws.activeIntegrations.slice(0, 6).map((ai) => {
                    const ProvSvg = getProviderIcon(ai.provider);
                    return (
                      <div
                        key={ai.integrationId}
                        className="w-7 h-7 rounded-lg bg-muted/60 border border-border flex items-center justify-center"
                        title={`${ai.integrationName} (${ai.provider})`}
                      >
                        {ProvSvg ? <ProvSvg size={16} /> : <Plug className="w-3.5 h-3.5 text-muted-foreground" />}
                      </div>
                    );
                  })}
                  {ws.activeIntegrations.length > 6 && (
                    <span className="text-[10px] text-muted-foreground ml-1">+{ws.activeIntegrations.length - 6}</span>
                  )}
                </div>
              )}

              {/* Badges */}
              <div className="flex items-center gap-2">
                {ws.isDefault && (
                  <span className="text-[9px] font-medium text-primary bg-primary/10 px-2 py-0.5 rounded-full">Default</span>
                )}
                {ws.id === activeWorkspace?.id && (
                  <span className="text-[9px] font-medium text-emerald-600 bg-emerald-500/10 px-2 py-0.5 rounded-full flex items-center gap-1">
                    <Check className="w-2.5 h-2.5" />Active
                  </span>
                )}
              </div>
            </div>

            {/* Enter Workspace Button */}
            {onEnterWorkspace && (
              <motion.button
                whileTap={{ scale: 0.97 }}
                onClick={(e) => { e.stopPropagation(); onEnterWorkspace(ws); }}
                className="w-full flex items-center justify-center gap-2 mt-3 h-9 rounded-lg bg-primary/5 border border-primary/10 text-xs font-medium text-primary hover:bg-primary/10 hover:border-primary/20 transition-all group"
              >
                Enter Workspace
                <ArrowRight className="w-3.5 h-3.5 group-hover:translate-x-0.5 transition-transform" />
              </motion.button>
            )}
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

      {/* ─── Manage Integrations for Workspace Dialog ─── */}
      <AnimatePresence>
        {manageWs && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 z-50 flex items-center justify-center">
            <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={() => setManageWsId(null)} />
            <motion.div
              initial={{ opacity: 0, scale: 0.95, y: 10 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.95, y: 10 }}
              className="relative w-full max-w-md bg-card border border-border rounded-2xl shadow-2xl overflow-hidden"
            >
              <div className="flex items-center justify-between p-5 border-b border-border">
                <div>
                  <h3 className="text-sm font-semibold text-foreground">{manageWs.name} — Integrations</h3>
                  <p className="text-[11px] text-muted-foreground">Toggle integrations for this workspace</p>
                </div>
                <button onClick={() => setManageWsId(null)} className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted transition-colors">
                  <X className="w-4 h-4" />
                </button>
              </div>
              <div className="p-4 max-h-80 overflow-y-auto space-y-2">
                {activeIntegrations.length === 0 ? (
                  <div className="py-8 text-center text-sm text-muted-foreground">
                    No active integrations to link. Setup your pending integrations first.
                  </div>
                ) : (
                  activeIntegrations.map((int) => {
                    const isLinked = manageWs.activeIntegrations?.some((ai) => ai.integrationId === int.id);
                    const ProvSvg = getProviderIcon(int.provider);
                    const pInfo = getProviderInfo(int.provider);
                    return (
                      <div key={int.id} className="flex items-center gap-3 p-3 rounded-lg border border-border hover:border-primary/15 transition-all">
                        <div className="w-9 h-9 rounded-lg bg-muted/60 flex items-center justify-center shrink-0">
                          {ProvSvg ? <ProvSvg size={18} /> : <Plug className="w-4 h-4 text-muted-foreground" />}
                        </div>
                        <div className="flex-1 min-w-0">
                          <p className="text-sm font-medium text-foreground truncate">{int.name}</p>
                          <p className="text-[10px] text-muted-foreground">{pInfo?.description || int.provider}</p>
                        </div>
                        <button
                          onClick={() => handleToggle(manageWs.id, int.id, !isLinked)}
                          disabled={toggling === int.id}
                          className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium transition-all ${
                            isLinked
                              ? "text-emerald-700 bg-emerald-500/10 hover:bg-red-500/10 hover:text-red-600"
                              : "text-primary bg-primary/10 hover:bg-primary/20"
                          }`}
                        >
                          {toggling === int.id ? (
                            <Loader2 className="w-3.5 h-3.5 animate-spin" />
                          ) : isLinked ? (
                            <><Check className="w-3.5 h-3.5" /> Linked</>
                          ) : (
                            <><Link2 className="w-3.5 h-3.5" /> Link</>
                          )}
                        </button>
                      </div>
                    );
                  })
                )}
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
};

export default WorkspacesPanel;


