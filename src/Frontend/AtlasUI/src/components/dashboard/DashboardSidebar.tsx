import { motion } from "framer-motion";
import {
  LayoutDashboard,
  Plug,
  FolderOpen,
  Sparkles,
  Crown,
  Plus,
  Star,
  AlertCircle,
  GitBranch,
  Figma,
  Shield,
  BarChart3,
  Users,
  Terminal,
  Code2,
  Coffee,
  Container,
  Bug,
  FileCode,
  TrendingUp,
} from "lucide-react";
import { WorkspaceDto, IntegrationDto } from "@/services/api";
import { useAuth, UserRole } from "@/context/AuthContext";
import { getProviderIcon } from "@/components/icons/IntegrationIcons";
import { AnimatePresence } from "framer-motion";
import { ChevronLeft, ChevronRight } from "lucide-react";

interface DashboardSidebarProps {
  workspaces: WorkspaceDto[];
  integrations: IntegrationDto[];
  pendingIntegrations: IntegrationDto[];
  activeWorkspace: WorkspaceDto | null;
  onSwitchWorkspace: (ws: WorkspaceDto) => void;
  onCreateWorkspace: () => void;
  activeTab: string;
  onTabChange: (tab: string) => void;
  collapsed: boolean;
  onToggle: () => void;
}

const roleIcons: Record<UserRole, typeof LayoutDashboard> = {
  developer: GitBranch,
  designer: Figma,
  cybersecurity: Shield,
  marketer: BarChart3,
  "team-leader": Users,
};

const statusColors: Record<string, string> = {
  Active: "bg-emerald-500",
  PendingSetup: "bg-amber-500",
  Expired: "bg-red-500",
  Error: "bg-red-500",
  Disconnected: "bg-zinc-400",
};

const DashboardSidebar = ({
  workspaces,
  integrations,
  pendingIntegrations,
  activeWorkspace,
  onSwitchWorkspace,
  onCreateWorkspace,
  activeTab,
  onTabChange,
  collapsed,
  onToggle,
}: DashboardSidebarProps) => {
  const { user } = useAuth();
  const RoleIcon = user?.role ? roleIcons[user.role] : LayoutDashboard;
  const allIntegrations = [...integrations, ...pendingIntegrations];

  const navItems = [
    { id: "overview", icon: LayoutDashboard, label: "Overview" },
    { id: "workspaces", icon: FolderOpen, label: "Workspaces", badge: workspaces.length.toString() },
    { id: "integrations", icon: Plug, label: "Integrations", badge: allIntegrations.length.toString() },
  ];

  // Developer-specific nav items
  const devNavItems = user?.role === "developer" ? [
    { id: "dev-overview", icon: LayoutDashboard, label: "Dev Overview" },
    { id: "insights", icon: TrendingUp, label: "Insights" },
    { id: "utilities", icon: Terminal, label: "Utilities" },
    { id: "ai-agents", icon: Sparkles, label: "AI Agents", highlight: true },
    { id: "scripts", icon: Code2, label: "Scripts" },
    { id: "snippets", icon: FileCode, label: "Snippets" },
    { id: "focus", icon: Coffee, label: "Focus Timer" },
    { id: "docker", icon: Container, label: "Docker" },
    { id: "git-jira", icon: GitBranch, label: "Git & Jira" },
    { id: "monitoring", icon: Bug, label: "Monitoring" },
  ] : [];

  const aiNavItem = user?.role !== "developer" ? [
    { id: "ai", icon: Sparkles, label: "AI Assistant", highlight: true },
  ] : [];

  return (
    <motion.aside
      animate={{ width: collapsed ? 60 : 240 }}
      transition={{ duration: 0.25, ease: "easeInOut" }}
      className="shrink-0 border-r border-border bg-card/50 flex flex-col h-full overflow-hidden relative"
    >
      {/* Collapse Toggle */}
      <button
        onClick={onToggle}
        className="absolute -right-3 top-4 z-10 w-6 h-6 rounded-full bg-card border border-border flex items-center justify-center text-muted-foreground hover:text-foreground hover:bg-muted transition-colors shadow-sm"
      >
        {collapsed ? <ChevronRight className="w-3 h-3" /> : <ChevronLeft className="w-3 h-3" />}
      </button>
      {/* Nav */}
      <div className="p-3 flex-1 overflow-y-auto">
        <nav className="space-y-0.5 mb-5">
          {navItems.map((item, i) => (
            <motion.button
              key={item.id}
              initial={{ opacity: 0, x: -15 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ delay: i * 0.04 }}
              whileHover={{ x: collapsed ? 0 : 2 }}
              onClick={() => onTabChange(item.id)}
              title={collapsed ? item.label : undefined}
              className={`w-full flex items-center gap-2.5 ${collapsed ? "justify-center px-0 py-2.5" : "px-3 py-2"} rounded-lg text-[13px] transition-all ${
                activeTab === item.id
                  ? "bg-primary/10 text-primary font-medium shadow-sm shadow-primary/5"
                  : "text-muted-foreground hover:bg-muted hover:text-foreground"
              }`}
            >
              <item.icon className="w-4 h-4 shrink-0" />
              {!collapsed && <span className="flex-1 text-left truncate">{item.label}</span>}
              {!collapsed && item.badge && (
                <span className="text-[10px] bg-muted text-muted-foreground px-1.5 py-0.5 rounded font-medium">
                  {item.badge}
                </span>
              )}
            </motion.button>
          ))}
          {aiNavItem.map((item) => (
            <motion.button
              key={item.id}
              whileHover={{ x: collapsed ? 0 : 2 }}
              onClick={() => onTabChange(item.id)}
              title={collapsed ? item.label : undefined}
              className={`w-full flex items-center gap-2.5 ${collapsed ? "justify-center px-0 py-2.5" : "px-3 py-2"} rounded-lg text-[13px] transition-all ${
                activeTab === item.id
                  ? "bg-primary/10 text-primary font-medium shadow-sm shadow-primary/5"
                  : "text-primary/80 hover:bg-primary/5"
              }`}
            >
              <item.icon className="w-4 h-4 shrink-0" />
              {!collapsed && <span className="flex-1 text-left">{item.label}</span>}
            </motion.button>
          ))}

          {/* Developer-specific nav */}
          {devNavItems.length > 0 && (
            <>
              {!collapsed && (
                <div className="pt-3 pb-1 px-3">
                  <div className="h-px bg-border" />
                  <p className="text-[9px] font-semibold text-muted-foreground tracking-widest uppercase mt-2">Developer Tools</p>
                </div>
              )}
              {collapsed && <div className="my-2 mx-2 h-px bg-border" />}
              {devNavItems.map((item, i) => (
                <motion.button
                  key={item.id}
                  initial={{ opacity: 0, x: -15 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 0.15 + i * 0.03 }}
                  whileHover={{ x: collapsed ? 0 : 2 }}
                  onClick={() => onTabChange(item.id)}
                  title={collapsed ? item.label : undefined}
                  className={`w-full flex items-center gap-2.5 ${collapsed ? "justify-center px-0 py-2.5" : "px-3 py-2"} rounded-lg text-[13px] transition-all ${
                    activeTab === item.id
                      ? "bg-primary/10 text-primary font-medium shadow-sm shadow-primary/5"
                      : item.highlight
                      ? "text-primary/80 hover:bg-primary/5"
                      : "text-muted-foreground hover:bg-muted hover:text-foreground"
                  }`}
                >
                  <item.icon className="w-4 h-4 shrink-0" />
                  {!collapsed && <span className="flex-1 text-left truncate">{item.label}</span>}
                </motion.button>
              ))}
            </>
          )}
        </nav>

        {/* Workspaces Quick List */}
        {!collapsed && (
          <div className="mb-5">
            <div className="flex items-center justify-between px-3 mb-1.5">
              <p className="text-[10px] font-semibold text-muted-foreground tracking-widest uppercase">
                Workspaces
              </p>
              <button
                onClick={onCreateWorkspace}
                className="w-5 h-5 rounded flex items-center justify-center text-muted-foreground hover:text-primary hover:bg-primary/10 transition-colors"
              >
                <Plus className="w-3.5 h-3.5" />
              </button>
            </div>
            <div className="space-y-0.5">
              {workspaces.slice(0, 5).map((ws) => (
                <motion.button
                  key={ws.id}
                  whileHover={{ x: 2 }}
                  onClick={() => onSwitchWorkspace(ws)}
                  className={`w-full flex items-center gap-2 px-3 py-1.5 rounded-lg text-[12px] transition-all ${
                    ws.id === activeWorkspace?.id
                      ? "bg-primary/8 text-primary font-medium"
                      : "text-muted-foreground hover:bg-muted hover:text-foreground"
                  }`}
                >
                  {ws.isDefault ? (
                    <Star className="w-3 h-3 text-amber-500 fill-amber-500 shrink-0" />
                  ) : (
                    <div className="w-3 h-3 rounded border border-border shrink-0" />
                  )}
                  <span className="truncate flex-1 text-left">{ws.name}</span>
                  {ws.activeIntegrations && ws.activeIntegrations.length > 0 && (
                    <span className="text-[9px] text-muted-foreground">{ws.activeIntegrations.length}</span>
                  )}
                </motion.button>
              ))}
            </div>
          </div>
        )}

        {/* Active Integrations */}
        {!collapsed && (
          <div className="mb-5">
            <p className="text-[10px] font-semibold text-muted-foreground tracking-widest uppercase px-3 mb-1.5">
              Integrations
            </p>
            <div className="space-y-0.5">
              {allIntegrations.slice(0, 6).map((int) => {
                const ProviderSvg = getProviderIcon(int.provider);
                return (
                  <motion.button
                    key={int.id}
                    whileHover={{ x: 2 }}
                    onClick={() => onTabChange("integrations")}
                    className="w-full flex items-center gap-2 px-3 py-1.5 rounded-lg text-[12px] text-muted-foreground hover:bg-muted hover:text-foreground transition-all"
                  >
                    <span className="shrink-0 w-4 h-4 flex items-center justify-center">
                      {ProviderSvg ? <ProviderSvg size={14} /> : <div className={`w-1.5 h-1.5 rounded-full ${statusColors[int.status] || "bg-zinc-400"}`} />}
                    </span>
                    <span className="truncate flex-1 text-left">{int.name}</span>
                    <div className={`w-1.5 h-1.5 rounded-full shrink-0 ${statusColors[int.status] || "bg-zinc-400"}`} />
                  </motion.button>
                );
              })}
              {pendingIntegrations.length > 0 && (
                <motion.button
                  whileHover={{ x: 2 }}
                  onClick={() => onTabChange("integrations")}
                  className="w-full flex items-center gap-2 px-3 py-1.5 rounded-lg text-[12px] text-amber-600 hover:bg-amber-500/10 transition-all"
                >
                  <AlertCircle className="w-3.5 h-3.5" />
                  <span>{pendingIntegrations.length} pending setup</span>
                </motion.button>
              )}
            </div>
          </div>
        )}

        {/* Role Badge */}
        <div className={collapsed ? "px-1" : "px-3"}>
          <div className={`flex items-center ${collapsed ? "justify-center" : "gap-2 px-3"} py-2 rounded-lg bg-primary/5 border border-primary/10`}>
            <RoleIcon className="w-4 h-4 text-primary" />
            {!collapsed && <span className="text-[11px] font-medium text-primary capitalize">{user?.role?.replace("-", " ") || "User"}</span>}
          </div>
        </div>
      </div>

      {/* Upgrade CTA */}
      <div className={`${collapsed ? "p-1.5" : "p-3"} border-t border-border`}>
        <motion.button
          whileHover={{ scale: 1.01, boxShadow: "0 4px 20px -4px hsl(var(--primary) / 0.3)" }}
          whileTap={{ scale: 0.99 }}
          className={`w-full flex items-center justify-center gap-2 ${collapsed ? "h-9 rounded-lg" : "h-9 rounded-lg"} bg-gradient-to-r from-primary to-primary/80 text-primary-foreground text-xs font-medium shadow-md shadow-primary/20 transition-all`}
        >
          <Crown className="w-3.5 h-3.5" />
          {!collapsed && "Upgrade to Pro"}
        </motion.button>
      </div>
    </motion.aside>
  );
};

export default DashboardSidebar;

