import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { ArrowLeft, Maximize2, Minimize2, Sparkles, X, FolderOpen, Plug, FileText, Key, Link2, ExternalLink, Clock, Users, MessageSquare } from "lucide-react";
import { WorkspaceDto, IntegrationDto } from "@/services/api";
import { useAuth } from "@/context/AuthContext";
import { getProviderIcon } from "@/components/icons/IntegrationIcons";

interface WorkspaceImmersiveViewProps {
  workspace: WorkspaceDto;
  integrations: IntegrationDto[];
  onExit: () => void;
}

// ─── Skeleton Loader ─────────────────────────────────────────────
const SkeletonBlock = ({ className }: { className?: string }) => (
  <div className={`rounded-lg bg-muted/50 animate-pulse ${className}`} />
);

const SkeletonView = ({ name: _name }: { name: string }) => (
  <div className="flex h-full">
    <div className="w-64 border-r border-border p-4 space-y-3">
      <SkeletonBlock className="h-6 w-32" />
      <SkeletonBlock className="h-4 w-full" />
      <SkeletonBlock className="h-4 w-3/4" />
      <SkeletonBlock className="h-4 w-5/6" />
      <SkeletonBlock className="h-4 w-2/3" />
    </div>
    <div className="flex-1 p-6 space-y-4">
      <SkeletonBlock className="h-8 w-48" />
      <SkeletonBlock className="h-4 w-96" />
      <div className="grid grid-cols-3 gap-4 mt-6">
        <SkeletonBlock className="h-24" />
        <SkeletonBlock className="h-24" />
        <SkeletonBlock className="h-24" />
      </div>
      <SkeletonBlock className="h-48 mt-4" />
    </div>
    <div className="w-72 border-l border-border p-4 space-y-3">
      <SkeletonBlock className="h-6 w-24" />
      <SkeletonBlock className="h-12 w-full" />
      <SkeletonBlock className="h-12 w-full" />
      <SkeletonBlock className="h-12 w-full" />
    </div>
  </div>
);

// ─── Left Context Panel ──────────────────────────────────────────
const ContextPanel = ({ workspace, integrations: _integrations }: { workspace: WorkspaceDto; integrations: IntegrationDto[] }) => {
  const wsIntegrations = workspace.activeIntegrations || [];

  return (
    <div className="w-64 shrink-0 border-r border-border bg-card/30 overflow-y-auto">
      <div className="p-4 border-b border-border">
        <div className="flex items-center gap-2 mb-1">
          <FolderOpen className="w-4 h-4 text-primary" />
          <h3 className="text-sm font-semibold text-foreground truncate">{workspace.name}</h3>
        </div>
        {workspace.description && (
          <p className="text-[11px] text-muted-foreground">{workspace.description}</p>
        )}
        {workspace.localFolderPath && (
          <p className="text-[10px] text-muted-foreground/60 font-mono mt-1 truncate">{workspace.localFolderPath}</p>
        )}
      </div>

      {/* Connected Integrations */}
      <div className="p-3">
        <p className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider mb-2 flex items-center gap-1.5">
          <Plug className="w-3 h-3" /> Integrations ({wsIntegrations.length})
        </p>
        <div className="space-y-1">
          {wsIntegrations.map((wi) => {
            const ProviderSvg = getProviderIcon(wi.provider);
            return (
              <div key={wi.integrationId} className="flex items-center gap-2 px-2 py-1.5 rounded-lg hover:bg-muted/30 transition-colors">
                <span className="w-5 h-5 flex items-center justify-center shrink-0">
                  {ProviderSvg ? <ProviderSvg size={14} /> : <Plug className="w-3.5 h-3.5 text-muted-foreground" />}
                </span>
                <span className="text-xs text-foreground truncate">{wi.integrationName}</span>
                <div className={`w-1.5 h-1.5 rounded-full shrink-0 ${wi.enabled ? "bg-emerald-500" : "bg-zinc-400"}`} />
              </div>
            );
          })}
          {wsIntegrations.length === 0 && (
            <p className="text-[10px] text-muted-foreground py-2">No integrations linked</p>
          )}
        </div>
      </div>

      {/* Quick Links */}
      <div className="p-3 border-t border-border">
        <p className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider mb-2 flex items-center gap-1.5">
          <Link2 className="w-3 h-3" /> Quick Links
        </p>
        <div className="space-y-1">
          {[
            { label: "API Documentation", icon: FileText },
            { label: "Environment Variables", icon: Key },
            { label: "Figma Designs", icon: ExternalLink },
          ].map((link) => (
            <button key={link.label} className="w-full flex items-center gap-2 px-2 py-1.5 rounded-lg text-xs text-muted-foreground hover:bg-muted/30 hover:text-foreground transition-colors">
              <link.icon className="w-3.5 h-3.5 shrink-0" />
              <span className="truncate">{link.label}</span>
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};

// ─── Center Stage ────────────────────────────────────────────────
const CenterStage = ({ workspace }: { workspace: WorkspaceDto }) => {
  const { user } = useAuth();

  return (
    <div className="flex-1 overflow-y-auto p-6">
      <div className="max-w-4xl mx-auto space-y-6">
        {/* Workspace Header */}
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl border border-primary/8 p-6"
        >
          <div className="absolute inset-0 bg-gradient-to-br from-primary/[0.05] via-transparent to-primary/[0.02]" />
          <div className="absolute top-0 right-0 w-64 h-64 bg-primary/[0.03] rounded-full blur-[60px] -translate-y-1/2 translate-x-1/4" />
          <div className="relative">
            <h2 className="text-xl font-bold text-foreground tracking-tight">{workspace.name}</h2>
            <p className="text-sm text-muted-foreground mt-1">
              {workspace.description || "Your workspace is ready. Start building."}
            </p>
          </div>
        </motion.div>

        {/* Stats */}
        <div className="grid grid-cols-3 gap-3">
          {[
            { label: "Integrations", value: workspace.activeIntegrations?.length || 0, icon: Plug, gradient: "from-blue-500/12 to-cyan-500/5" },
            { label: "Status", value: workspace.isDefault ? "Default" : "Active", icon: FolderOpen, gradient: "from-emerald-500/12 to-green-500/5" },
            { label: "Shared", value: workspace.isShared ? "Yes" : "Private", icon: Users, gradient: "from-violet-500/12 to-purple-500/5" },
          ].map((stat, i) => (
            <motion.div
              key={stat.label}
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.1 + i * 0.06 }}
              className={`relative overflow-hidden rounded-xl border border-border bg-gradient-to-br ${stat.gradient} p-4 group`}
            >
              <div className="absolute top-0 right-0 w-16 h-16 bg-primary/[0.03] rounded-full -translate-y-1/2 translate-x-1/3 group-hover:bg-primary/[0.05] transition-colors duration-500" />
              <stat.icon className="w-4 h-4 text-muted-foreground mb-2" />
              <p className="text-xl font-bold text-foreground">{stat.value}</p>
              <p className="text-[10px] text-muted-foreground mt-0.5">{stat.label}</p>
            </motion.div>
          ))}
        </div>

        {/* Role-specific content placeholder */}
        <motion.div
          initial={{ opacity: 0, y: 15 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.2 }}
          className="relative overflow-hidden rounded-xl border border-border p-6 bg-gradient-to-br from-card to-card/60"
        >
          <div className="absolute top-0 right-0 w-40 h-40 bg-primary/[0.02] rounded-full -translate-y-1/2 translate-x-1/3" />
          <h3 className="text-sm font-semibold text-foreground mb-4 relative">
            {user?.role === "developer" ? "🖥 Active Tasks & Code" :
             user?.role === "designer" ? "🎨 Design Files & Assets" :
             user?.role === "cybersecurity" ? "🛡 Security Scans & Alerts" :
             "📋 Project Board"}
          </h3>
          <div className="grid grid-cols-2 gap-3 relative">
            {[1, 2, 3, 4].map((i) => (
              <div key={i} className="p-4 rounded-lg bg-muted/20 border border-border/50 hover:bg-muted/30 transition-colors">
                <div className="w-20 h-3 bg-muted/40 rounded mb-2.5 animate-pulse" />
                <div className="w-full h-2 bg-muted/30 rounded mb-1.5" />
                <div className="w-3/4 h-2 bg-muted/30 rounded" />
              </div>
            ))}
          </div>
          <p className="text-[11px] text-muted-foreground mt-5 text-center relative">
            Workspace-specific content will populate as you connect integrations and start working.
          </p>
        </motion.div>
      </div>
    </div>
  );
};

// ─── Right Omni-Feed ─────────────────────────────────────────────
const OmniFeed = () => {
  const feedItems = [
    { id: 1, user: "System", action: "Workspace opened", time: "Just now", icon: FolderOpen },
    { id: 2, user: "Atlas AI", action: "Integration sync completed", time: "2m ago", icon: Sparkles },
    { id: 3, user: "GitHub", action: "New commit pushed to main", time: "15m ago", icon: MessageSquare },
    { id: 4, user: "Team", action: "Sprint planning scheduled", time: "1h ago", icon: Users },
    { id: 5, user: "Sentry", action: "Error rate decreased by 12%", time: "3h ago", icon: Clock },
  ];

  return (
    <div className="w-72 shrink-0 border-l border-border bg-card/30 overflow-y-auto">
      <div className="p-4 border-b border-border">
        <h3 className="text-xs font-semibold text-foreground flex items-center gap-1.5">
          <MessageSquare className="w-3.5 h-3.5 text-primary" />
          Activity Feed
        </h3>
      </div>
      <div className="p-2">
        {feedItems.map((item, i) => (
          <motion.div
            key={item.id}
            initial={{ opacity: 0, x: 10 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.5 + i * 0.1 }}
            className="flex items-start gap-2.5 p-2.5 rounded-lg hover:bg-muted/30 transition-colors"
          >
            <div className="w-7 h-7 rounded-lg bg-primary/10 flex items-center justify-center shrink-0 mt-0.5">
              <item.icon className="w-3.5 h-3.5 text-primary" />
            </div>
            <div className="min-w-0">
              <p className="text-[11px] text-foreground"><span className="font-medium">{item.user}</span> {item.action}</p>
              <p className="text-[9px] text-muted-foreground mt-0.5">{item.time}</p>
            </div>
          </motion.div>
        ))}
      </div>
    </div>
  );
};

// ─── AI Co-Pilot Popup ───────────────────────────────────────────
const AICoPilot = ({ workspaceName, onDismiss }: { workspaceName: string; onDismiss: () => void }) => (
  <motion.div
    initial={{ opacity: 0, y: 20, scale: 0.9 }}
    animate={{ opacity: 1, y: 0, scale: 1 }}
    exit={{ opacity: 0, y: 20, scale: 0.9 }}
    className="fixed bottom-6 right-6 z-50 w-80 bg-card border border-border rounded-2xl shadow-2xl shadow-black/10 overflow-hidden"
  >
    <div className="p-4">
      <div className="flex items-start gap-3">
        <div className="w-9 h-9 rounded-xl bg-gradient-to-br from-primary to-primary/60 flex items-center justify-center shrink-0 shadow-lg shadow-primary/20">
          <Sparkles className="w-5 h-5 text-primary-foreground" />
        </div>
        <div className="flex-1 min-w-0">
          <p className="text-sm font-medium text-foreground">Welcome to {workspaceName}! 👋</p>
          <p className="text-[11px] text-muted-foreground mt-1">
            I'm your AI co-pilot. Need help reviewing PRs, running scripts, or finding anything? Just ask!
          </p>
        </div>
        <button onClick={onDismiss} className="w-6 h-6 rounded flex items-center justify-center text-muted-foreground hover:text-foreground transition-colors shrink-0">
          <X className="w-3.5 h-3.5" />
        </button>
      </div>
    </div>
  </motion.div>
);

// ─── Main Immersive View ─────────────────────────────────────────
const WorkspaceImmersiveView = ({ workspace, integrations, onExit }: WorkspaceImmersiveViewProps) => {
  const [loading, setLoading] = useState(true);
  const [zenMode, setZenMode] = useState(false);
  const [showCoPilot, setShowCoPilot] = useState(false);

  // Skeleton loading simulation + data fetch
  useEffect(() => {
    const timer = setTimeout(() => {
      setLoading(false);
      // Show AI co-pilot after data loads
      setTimeout(() => setShowCoPilot(true), 300);
    }, 600);
    return () => clearTimeout(timer);
  }, []);

  // Auto-dismiss co-pilot after 8s
  useEffect(() => {
    if (showCoPilot) {
      const timer = setTimeout(() => setShowCoPilot(false), 8000);
      return () => clearTimeout(timer);
    }
  }, [showCoPilot]);

  // Escape to exit, Z for zen
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if (e.key === "Escape") onExit();
      if (e.key === "z" && !e.ctrlKey && !e.metaKey && !(e.target instanceof HTMLInputElement) && !(e.target instanceof HTMLTextAreaElement)) {
        setZenMode((z) => !z);
      }
    };
    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [onExit]);

  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.96 }}
      animate={{ opacity: 1, scale: 1 }}
      exit={{ opacity: 0, scale: 0.96 }}
      transition={{ duration: 0.35, ease: "easeOut" }}
      className="fixed inset-0 z-40 bg-background flex flex-col"
    >
      {/* Top Bar */}
      <div className="h-12 border-b border-border bg-card/60 backdrop-blur-2xl flex items-center justify-between px-4 shrink-0">
        <div className="flex items-center gap-3">
          <motion.button
            whileHover={{ x: -2 }}
            whileTap={{ scale: 0.95 }}
            onClick={onExit}
            className="flex items-center gap-1.5 text-sm text-muted-foreground hover:text-foreground transition-colors"
          >
            <ArrowLeft className="w-4 h-4" />
            <span className="text-xs">Back</span>
          </motion.button>
          <div className="h-4 w-px bg-border" />
          <div className="flex items-center gap-2">
            <div className="w-2 h-2 rounded-full bg-primary animate-pulse" />
            <span className="text-sm font-medium text-foreground">{workspace.name}</span>
            {workspace.isDefault && <span className="text-[9px] bg-primary/10 text-primary px-1.5 py-0.5 rounded-full">Default</span>}
          </div>
        </div>
        <div className="flex items-center gap-2">
          <motion.button
            whileTap={{ scale: 0.95 }}
            onClick={() => setZenMode(!zenMode)}
            className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium transition-all ${
              zenMode ? "bg-primary text-primary-foreground" : "text-muted-foreground hover:bg-muted hover:text-foreground"
            }`}
          >
            {zenMode ? <Minimize2 className="w-3.5 h-3.5" /> : <Maximize2 className="w-3.5 h-3.5" />}
            {zenMode ? "Exit Zen" : "Zen Mode"}
          </motion.button>
          <span className="text-[9px] text-muted-foreground bg-muted px-1.5 py-0.5 rounded font-mono">ESC to exit · Z for zen</span>
        </div>
      </div>

      {/* Main Content */}
      {loading ? (
        <SkeletonView name={workspace.name} />
      ) : (
        <div className="flex flex-1 overflow-hidden">
          {/* Left Panel */}
          <AnimatePresence>
            {!zenMode && (
              <motion.div
                initial={{ width: 256, opacity: 1 }}
                exit={{ width: 0, opacity: 0 }}
                animate={{ width: 256, opacity: 1 }}
                transition={{ duration: 0.3 }}
                className="overflow-hidden"
              >
                <ContextPanel workspace={workspace} integrations={integrations} />
              </motion.div>
            )}
          </AnimatePresence>

          {/* Center */}
          <CenterStage workspace={workspace} />

          {/* Right Panel */}
          <AnimatePresence>
            {!zenMode && (
              <motion.div
                initial={{ width: 288, opacity: 1 }}
                exit={{ width: 0, opacity: 0 }}
                animate={{ width: 288, opacity: 1 }}
                transition={{ duration: 0.3 }}
                className="overflow-hidden"
              >
                <OmniFeed />
              </motion.div>
            )}
          </AnimatePresence>
        </div>
      )}

      {/* AI Co-Pilot */}
      <AnimatePresence>
        {showCoPilot && <AICoPilot workspaceName={workspace.name} onDismiss={() => setShowCoPilot(false)} />}
      </AnimatePresence>
    </motion.div>
  );
};

export default WorkspaceImmersiveView;

