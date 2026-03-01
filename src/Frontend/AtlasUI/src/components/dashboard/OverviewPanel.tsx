import { motion } from "framer-motion";
import {
  FolderOpen,
  Plug,
  Zap,
  ArrowRight,
  Plus,
  Star,
  Clock,
  AlertCircle,
} from "lucide-react";
import { WorkspaceDto, IntegrationDto } from "@/services/api";
import { useAuth } from "@/context/AuthContext";
import { getProviderInfo } from "@/lib/integration-providers";
import { getProviderIcon } from "@/components/icons/IntegrationIcons";

interface OverviewPanelProps {
  workspaces: WorkspaceDto[];
  integrations: IntegrationDto[];
  pendingIntegrations: IntegrationDto[];
  activeWorkspace: WorkspaceDto | null;
  onTabChange: (tab: string) => void;
  onCreateWorkspace: () => void;
}

const statusDot: Record<string, string> = {
  Active: "bg-emerald-500",
  PendingSetup: "bg-amber-500",
  Expired: "bg-red-500",
  Error: "bg-red-500",
  Disconnected: "bg-zinc-400",
};

const OverviewPanel = ({
  workspaces,
  integrations,
  pendingIntegrations,
  onTabChange,
  onCreateWorkspace,
}: OverviewPanelProps) => {
  const { user } = useAuth();
  const displayName = user?.fullName?.split(" ")[0] || "User";
  const activeCount = integrations.filter((i) => i.status === "Active").length;
  const allIntegrations = [...integrations, ...pendingIntegrations];
  const now = new Date();
  const hour = now.getHours();
  const greeting = hour < 12 ? "Good morning" : hour < 18 ? "Good afternoon" : "Good evening";

  return (
    <div className="space-y-6">
      {/* Hero Greeting */}
      <motion.div
        initial={{ opacity: 0, y: 12 }}
        animate={{ opacity: 1, y: 0 }}
        className="relative overflow-hidden rounded-2xl border border-primary/8 p-6 md:p-8"
      >
        <div className="absolute inset-0 bg-gradient-to-br from-primary/[0.06] via-transparent to-primary/[0.03]" />
        <div className="absolute top-0 right-0 w-80 h-80 bg-primary/[0.04] rounded-full blur-[80px] -translate-y-1/2 translate-x-1/4" />
        <div className="absolute bottom-0 left-0 w-48 h-48 bg-blue-500/[0.03] rounded-full blur-[60px] translate-y-1/3 -translate-x-1/4" />
        <div className="relative">
          <h1 className="text-2xl font-bold text-foreground mb-1.5 tracking-tight">
            {greeting}, {displayName}! 👋
          </h1>
          <p className="text-sm text-muted-foreground max-w-lg leading-relaxed">
            You have <span className="text-foreground font-medium">{workspaces.length} workspace{workspaces.length !== 1 ? "s" : ""}</span> and{" "}
            <span className="text-foreground font-medium">{activeCount} active integration{activeCount !== 1 ? "s" : ""}</span>.
            {pendingIntegrations.length > 0 && (
              <span className="text-amber-600 font-medium"> {pendingIntegrations.length} integration{pendingIntegrations.length !== 1 ? "s" : ""} need setup.</span>
            )}
          </p>
        </div>
      </motion.div>

      {/* Stats Row */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-3">
        {[
          { label: "Workspaces", value: workspaces.length, icon: FolderOpen, gradient: "from-blue-500/12 to-cyan-500/5", iconColor: "text-blue-400" },
          { label: "Integrations", value: allIntegrations.length, icon: Plug, gradient: "from-violet-500/12 to-purple-500/5", iconColor: "text-violet-400" },
          { label: "Active", value: activeCount, icon: Zap, gradient: "from-emerald-500/12 to-green-500/5", iconColor: "text-emerald-400" },
          { label: "Pending Setup", value: pendingIntegrations.length, icon: AlertCircle, gradient: "from-amber-500/12 to-orange-500/5", iconColor: "text-amber-400" },
        ].map((stat, i) => (
          <motion.div
            key={stat.label}
            initial={{ opacity: 0, y: 15 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1 + i * 0.05 }}
            whileHover={{ y: -3, transition: { duration: 0.2 } }}
            className={`relative overflow-hidden rounded-xl border border-border bg-gradient-to-br ${stat.gradient} p-4 cursor-pointer group`}
          >
            <div className="absolute top-0 right-0 w-16 h-16 bg-primary/[0.03] rounded-full -translate-y-1/2 translate-x-1/3 group-hover:bg-primary/[0.05] transition-colors duration-500" />
            <stat.icon className={`w-4 h-4 ${stat.iconColor} mb-3`} />
            <p className="text-2xl font-bold text-foreground tracking-tight">{stat.value}</p>
            <p className="text-[10px] text-muted-foreground mt-0.5">{stat.label}</p>
          </motion.div>
        ))}
      </div>

      {/* Two Column Layout */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
        {/* Workspaces Card */}
        <motion.div
          initial={{ opacity: 0, y: 15 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.2 }}
          className="relative overflow-hidden rounded-xl border border-border bg-gradient-to-br from-card to-card/60"
        >
          <div className="absolute top-0 right-0 w-32 h-32 bg-primary/[0.02] rounded-full -translate-y-1/2 translate-x-1/3" />
          <div className="flex items-center justify-between p-4 border-b border-border relative">
            <div className="flex items-center gap-2">
              <FolderOpen className="w-4 h-4 text-primary" />
              <h3 className="text-sm font-semibold text-foreground">Workspaces</h3>
            </div>
            <button
              onClick={onCreateWorkspace}
              className="flex items-center gap-1 text-xs text-primary hover:text-primary/80 transition-colors"
            >
              <Plus className="w-3.5 h-3.5" />
              New
            </button>
          </div>
          <div className="divide-y divide-border">
            {workspaces.length === 0 ? (
              <div className="p-8 text-center">
                <FolderOpen className="w-8 h-8 text-muted-foreground/30 mx-auto mb-2" />
                <p className="text-sm text-muted-foreground">No workspaces yet</p>
                <button onClick={onCreateWorkspace} className="mt-2 text-xs text-primary hover:underline">Create your first workspace</button>
              </div>
            ) : (
              workspaces.slice(0, 4).map((ws) => (
                <div key={ws.id} className="flex items-center gap-3 p-3 hover:bg-muted/30 transition-colors">
                  <div className="w-9 h-9 rounded-lg bg-primary/10 flex items-center justify-center shrink-0">
                    {ws.isDefault ? <Star className="w-4 h-4 text-primary fill-primary/30" /> : <FolderOpen className="w-4 h-4 text-primary/60" />}
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-foreground truncate">{ws.name}</p>
                    <p className="text-[11px] text-muted-foreground">
                      {ws.activeIntegrations?.length || 0} integration{(ws.activeIntegrations?.length || 0) !== 1 ? "s" : ""}
                      {ws.isDefault && " · Default"}
                    </p>
                  </div>
                  <ArrowRight className="w-3.5 h-3.5 text-muted-foreground/40" />
                </div>
              ))
            )}
          </div>
          {workspaces.length > 4 && (
            <button onClick={() => onTabChange("workspaces")} className="w-full py-2.5 text-xs text-primary hover:bg-primary/5 transition-colors border-t border-border">
              View all {workspaces.length} workspaces
            </button>
          )}
        </motion.div>

        {/* Integrations Card */}
        <motion.div
          initial={{ opacity: 0, y: 15 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.25 }}
          className="relative overflow-hidden rounded-xl border border-border bg-gradient-to-br from-card to-card/60"
        >
          <div className="absolute top-0 right-0 w-32 h-32 bg-violet-500/[0.02] rounded-full -translate-y-1/2 translate-x-1/3" />
          <div className="flex items-center justify-between p-4 border-b border-border">
            <div className="flex items-center gap-2">
              <Plug className="w-4 h-4 text-primary" />
              <h3 className="text-sm font-semibold text-foreground">Integrations</h3>
            </div>
            <button
              onClick={() => onTabChange("integrations")}
              className="text-xs text-primary hover:text-primary/80 transition-colors"
            >
              Manage
            </button>
          </div>
          <div className="divide-y divide-border">
            {allIntegrations.length === 0 ? (
              <div className="p-8 text-center">
                <Plug className="w-8 h-8 text-muted-foreground/30 mx-auto mb-2" />
                <p className="text-sm text-muted-foreground">No integrations yet</p>
                <button onClick={() => onTabChange("integrations")} className="mt-2 text-xs text-primary hover:underline">Connect your first integration</button>
              </div>
            ) : (
              allIntegrations.slice(0, 5).map((int) => {
                const pInfo = getProviderInfo(int.provider);
                const ProviderSvg = getProviderIcon(int.provider);
                return (
                  <div key={int.id} className="flex items-center gap-3 p-3 hover:bg-muted/30 transition-colors">
                    <div className="w-9 h-9 rounded-lg bg-muted/50 flex items-center justify-center shrink-0">
                      {ProviderSvg ? <ProviderSvg size={20} /> : <Plug className="w-4 h-4 text-muted-foreground" />}
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-foreground truncate">{int.name}</p>
                      <p className="text-[11px] text-muted-foreground">{pInfo?.description || int.provider}</p>
                    </div>
                    <div className="flex items-center gap-1.5">
                      <div className={`w-1.5 h-1.5 rounded-full ${statusDot[int.status] || "bg-zinc-400"}`} />
                      <span className="text-[10px] text-muted-foreground">
                        {int.status === "PendingSetup" ? "Needs setup" : int.status}
                      </span>
                    </div>
                  </div>
                );
              })
            )}
          </div>
          {pendingIntegrations.length > 0 && (
            <div className="p-3 bg-amber-500/5 border-t border-amber-500/10">
              <div className="flex items-center gap-2 text-xs text-amber-600">
                <Clock className="w-3.5 h-3.5" />
                <span>{pendingIntegrations.length} integration{pendingIntegrations.length !== 1 ? "s" : ""} waiting for setup</span>
              </div>
            </div>
          )}
        </motion.div>
      </div>

      {/* Quick Actions */}
      <motion.div
        initial={{ opacity: 0, y: 15 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.3 }}
        className="grid grid-cols-1 sm:grid-cols-3 gap-3"
      >
        {[
          { label: "New Workspace", desc: "Create a project workspace", icon: FolderOpen, action: onCreateWorkspace, gradient: "from-blue-500/8 to-transparent" },
          { label: "Connect Tool", desc: "Add a new integration", icon: Plug, action: () => onTabChange("integrations"), gradient: "from-violet-500/8 to-transparent" },
          { label: "AI Assistant", desc: "Get help from Atlas AI", icon: Zap, action: () => onTabChange("ai"), gradient: "from-emerald-500/8 to-transparent" },
        ].map((qa) => (
          <motion.button
            key={qa.label}
            whileHover={{ y: -2, transition: { duration: 0.2 } }}
            whileTap={{ scale: 0.98 }}
            onClick={qa.action}
            className={`relative overflow-hidden flex items-center gap-3 p-4 rounded-xl border border-border bg-gradient-to-br ${qa.gradient} hover:border-primary/15 transition-all text-left group`}
          >
            <div className="absolute top-0 right-0 w-16 h-16 bg-primary/[0.02] rounded-full -translate-y-1/2 translate-x-1/3 group-hover:bg-primary/[0.04] transition-colors duration-500" />
            <div className="w-10 h-10 rounded-lg bg-card/80 border border-border/50 flex items-center justify-center shrink-0 group-hover:border-primary/20 transition-colors">
              <qa.icon className="w-5 h-5 text-foreground/70 group-hover:text-primary transition-colors" />
            </div>
            <div>
              <p className="text-sm font-medium text-foreground">{qa.label}</p>
              <p className="text-[10px] text-muted-foreground">{qa.desc}</p>
            </div>
          </motion.button>
        ))}
      </motion.div>
    </div>
  );
};

export default OverviewPanel;


