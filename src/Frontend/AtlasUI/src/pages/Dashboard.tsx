import { useState, useEffect, useCallback } from "react";
import { Navigate } from "react-router-dom";
import { motion, AnimatePresence } from "framer-motion";
import {
  Loader2, Search, Command, Eye, EyeOff, Bell, LogOut,
  Sun, Moon, ChevronDown, ChevronLeft, ChevronRight, User, Crown,
  LayoutDashboard, Plug, FolderOpen, Star, Plus,
  GitBranch, Terminal, Code2, Coffee,
  Container, Bug, FileCode, TrendingUp, SquareKanban, Sparkles,
  Activity, Radio, Trophy, PanelRightClose, PanelRightOpen,
  Bot, PenTool, Layers, Ruler, Palette,
  Shield, ShieldAlert, Zap, Wrench,
  DollarSign, FlaskConical, BarChart3,
  BookOpen, Radar, FolderGit2, Users, BookMarked, Settings2,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { useTheme } from "@/context/ThemeContext";
import { useWorkspaces } from "@/hooks/use-workspace";
import { WorkspaceDto, IntegrationDto, notificationsApi } from "@/services/api";
import { getProviderIcon } from "@/components/icons/IntegrationIcons";
import { useNavigate } from "react-router-dom";
import { startSignalR, stopSignalR, onNotification, offAll } from "@/services/signalr";
import { useToast } from "@/hooks/use-toast";

import CommandPalette from "@/components/dashboard/CommandPalette";
import OverviewPanel from "@/components/dashboard/OverviewPanel";
import WorkspacesPanel from "@/components/dashboard/WorkspacesPanel";
import IntegrationsPanel from "@/components/dashboard/IntegrationsPanel";
import CreateWorkspaceDialog from "@/components/dashboard/CreateWorkspaceDialog";
import WorkspaceImmersiveView from "@/components/dashboard/workspace/WorkspaceImmersiveView";
import DevOverviewPanel from "@/components/dashboard/developer/DevOverviewPanel";
import InsightsPanel from "@/components/dashboard/developer/InsightsPanel";
import UtilitiesPanel from "@/components/dashboard/developer/UtilitiesPanel";
import AIAgentsPanel from "@/components/dashboard/developer/AIAgentsPanel";
import ScriptsPanel from "@/components/dashboard/developer/ScriptsPanel";
import SnippetsPanel from "@/components/dashboard/developer/SnippetsPanel";
import FocusPanel from "@/components/dashboard/developer/FocusPanel";
import DockerPanel from "@/components/dashboard/developer/DockerPanel";
import GitHubPanel from "@/components/dashboard/developer/GitHubPanel";
import JiraPanel from "@/components/dashboard/developer/JiraPanel";
import MonitoringPanel from "@/components/dashboard/developer/MonitoringPanel";
import DesignerOverviewPanel from "@/components/dashboard/designer/DesignerOverviewPanel";
import DesignInsightsPanel from "@/components/dashboard/designer/DesignInsightsPanel";
import DesignUtilitiesPanel from "@/components/dashboard/designer/DesignUtilitiesPanel";
import FigmaPanel from "@/components/dashboard/designer/FigmaPanel";
import MiroPanel from "@/components/dashboard/designer/MiroPanel";
import LottiePanel from "@/components/dashboard/designer/LottiePanel";
import DribbblePanel from "@/components/dashboard/designer/DribbblePanel";
import ZeplinPanel from "@/components/dashboard/designer/ZeplinPanel";
import SecOpsOverviewPanel from "@/components/dashboard/cyber/SecOpsOverviewPanel";
import SecOpsInsightsPanel from "@/components/dashboard/cyber/SecOpsInsightsPanel";
import SecOpsUtilitiesPanel from "@/components/dashboard/cyber/SecOpsUtilitiesPanel";
import SecOpsAgentsPanel from "@/components/dashboard/cyber/SecOpsAgentsPanel";
import SecOpsScriptsPanel from "@/components/dashboard/cyber/SecOpsScriptsPanel";
import MarketerOverviewPanel from "@/components/dashboard/marketer/MarketerOverviewPanel";
import MarketerInsightsPanel from "@/components/dashboard/marketer/MarketerInsightsPanel";
import MarketerUtilitiesPanel from "@/components/dashboard/marketer/MarketerUtilitiesPanel";
import MarketerAgentsPanel from "@/components/dashboard/marketer/MarketerAgentsPanel";
import MarketerScriptsPanel from "@/components/dashboard/marketer/MarketerScriptsPanel";
import LeaderOverviewPanel from "@/components/dashboard/leader/LeaderOverviewPanel";
import LeaderInsightsPanel from "@/components/dashboard/leader/LeaderInsightsPanel";
import LeaderUtilitiesPanel from "@/components/dashboard/leader/LeaderUtilitiesPanel";
import LeaderAgentsPanel from "@/components/dashboard/leader/LeaderAgentsPanel";
import LeaderScriptsPanel from "@/components/dashboard/leader/LeaderScriptsPanel";
import LeaderModalsPanel from "@/components/dashboard/leader/LeaderModalsPanel";
import ProfilePanel from "@/components/dashboard/ProfilePanel";
import TeamsPanel from "@/components/dashboard/shared/TeamsPanel";
import OmniFeedPanel from "@/components/dashboard/shared/OmniFeedPanel";
import SquadRadarPanel from "@/components/dashboard/shared/SquadRadarPanel";
import SquadArenaPanel from "@/components/dashboard/shared/SquadArenaPanel";
import ResourceHubPanel from "@/components/dashboard/shared/ResourceHubPanel";
import KnowledgePanel from "@/components/dashboard/shared/KnowledgePanel";
import ProjectsPanel from "@/components/dashboard/shared/ProjectsPanel";
import NotificationsPanel from "@/components/dashboard/shared/NotificationsPanel";
import SettingsPanel from "@/components/dashboard/shared/SettingsPanel";

// ─── Types ────────────────────────────────────────────────────────
interface NavItem {
  id: string;
  icon: typeof LayoutDashboard;
  label: string;
  badge?: string;
  highlight?: boolean;
}

const statusColors: Record<string, string> = {
  Active: "bg-emerald-500",
  PendingSetup: "bg-amber-500",
  Expired: "bg-red-500",
  Error: "bg-red-500",
  Disconnected: "bg-zinc-500",
};

// ═══════════════════════════════════════════════════════════════════
// MAIN DASHBOARD — Premium 5 Zone Layout
// ═══════════════════════════════════════════════════════════════════
const Dashboard = () => {
  const { user, logout } = useAuth();
  const { theme, toggleTheme, setRole, currentRole, clearRole } = useTheme();
  const navigate = useNavigate();

  const [activeTab, setActiveTab] = useState(
    user?.role === "developer" ? "dev-overview" : user?.role === "designer" ? "designer-overview" : user?.role === "cybersecurity" ? "secops-overview" : user?.role === "marketer" ? "marketer-overview" : user?.role === "team-leader" ? "leader-overview" : "overview"
  );
  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false);
  const [rightPanelOpen, setRightPanelOpen] = useState(true);
  const [zenMode, setZenMode] = useState(false);
  const [enteredWorkspace, setEnteredWorkspace] = useState<WorkspaceDto | null>(null);
  const [wsDropdownOpen, setWsDropdownOpen] = useState(false);
  const [userMenuOpen, setUserMenuOpen] = useState(false);
  const [commandOpen, setCommandOpen] = useState(false);
  const [unreadCount, setUnreadCount] = useState(0);
  const { toast } = useToast();

  const {
    workspaces, integrations, pendingIntegrations, activeWorkspace,
    switchWorkspace, createWorkspace, deleteWorkspace, setDefaultWorkspace,
    toggleIntegration, loading, refresh,
  } = useWorkspaces();

  useEffect(() => { if (user?.role) setRole(user.role); }, [user?.role, setRole]);

  // Keyboard shortcuts
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if ((e.metaKey || e.ctrlKey) && e.key === "k") { e.preventDefault(); setCommandOpen(p => !p); }
      if (e.key === "Escape") { if (zenMode) setZenMode(false); if (commandOpen) setCommandOpen(false); }
    };
    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [zenMode, commandOpen]);

  // ── SignalR real-time connection ─────────────────────────────────
  useEffect(() => {
    startSignalR().catch(() => {});
    return () => { offAll(); stopSignalR(); };
  }, []);

  // ── Real-time notification listener ──────────────────────────────
  useEffect(() => {
    onNotification((data) => {
      setUnreadCount((prev) => prev + 1);
      toast({ title: data.title, description: data.body, duration: 4000 });
    });
  }, [toast]);

  // ── Fetch unread notification count ─────────────────────────────
  useEffect(() => {
    notificationsApi.getUnreadCount().then((res) => {
      if (res.data.isSuccess && res.data.data) setUnreadCount(res.data.data.total);
    }).catch(() => {});
  }, [activeTab]); // Refresh count when tab changes

  const handleLogout = async () => { clearRole(); await logout(); navigate("/login"); };

  if (user && !user.onboardingComplete) return <Navigate to="/onboarding" replace />;

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background">
        <motion.div initial={{ opacity: 0, scale: 0.9 }} animate={{ opacity: 1, scale: 1 }} className="flex flex-col items-center gap-5">
          <div className="relative">
            <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-primary to-primary/60 flex items-center justify-center shadow-xl shadow-primary/25">
              <span className="text-primary-foreground font-bold text-xl">A</span>
            </div>
            <Loader2 className="w-5 h-5 animate-spin text-primary absolute -bottom-1 -right-1" />
          </div>
          <div className="text-center">
            <p className="text-sm font-semibold text-foreground">Loading workspace...</p>
            <p className="text-xs text-muted-foreground mt-1">Connecting to your tools</p>
          </div>
        </motion.div>
      </div>
    );
  }

  const allIntegrations = [...integrations, ...pendingIntegrations];
  const role = user?.role;
  const coreNavItems: NavItem[] = [
    { id: role === "developer" ? "dev-overview" : role === "designer" ? "designer-overview" : role === "cybersecurity" ? "secops-overview" : role === "marketer" ? "marketer-overview" : role === "team-leader" ? "leader-overview" : "overview", icon: LayoutDashboard, label: "Overview" },
    { id: "workspaces", icon: FolderOpen, label: "Workspaces", badge: workspaces.length.toString() },
    { id: "integrations", icon: Plug, label: "Integrations", badge: allIntegrations.length.toString() },
    { id: "teams", icon: Users, label: "Teams" },
    { id: "omni-feed", icon: Activity, label: "OmniFeed" },
    { id: "squad-radar", icon: Radar, label: "Squad Radar" },
    { id: "squad-arena", icon: Trophy, label: "Squad Arena" },
    { id: "resource-hub", icon: BookOpen, label: "Resources" },
    { id: "knowledge", icon: BookMarked, label: "Knowledge" },
    // Projects → only developer
    ...(role === "developer" ? [{ id: "projects", icon: FolderGit2, label: "Projects" }] : []),
  ] as NavItem[];
  const devNavItems: NavItem[] = user?.role === "developer" ? [
    { id: "insights", icon: TrendingUp, label: "Insights" },
    { id: "utilities", icon: Terminal, label: "Utilities" },
    { id: "ai-agents", icon: Sparkles, label: "AI Agents", highlight: true },
    { id: "scripts", icon: Code2, label: "Scripts" },
    { id: "snippets", icon: FileCode, label: "Snippets" },
    { id: "focus", icon: Coffee, label: "Focus Timer" },
    { id: "docker", icon: Container, label: "Docker" },
    { id: "github", icon: GitBranch, label: "GitHub" },
    { id: "jira", icon: SquareKanban, label: "Jira" },
    { id: "monitoring", icon: Bug, label: "Monitoring" },
  ] : [];
  const designerNavItems: NavItem[] = user?.role === "designer" ? [
    { id: "design-insights", icon: TrendingUp, label: "Insights" },
    { id: "design-utilities", icon: Palette, label: "Design Tools" },
    { id: "figma", icon: PenTool, label: "Figma" },
    { id: "miro", icon: Layers, label: "Miro" },
    { id: "lottie", icon: Sparkles, label: "LottieFiles" },
    { id: "dribbble", icon: Eye, label: "Dribbble" },
    { id: "zeplin", icon: Ruler, label: "Zeplin" },
  ] : [];
  const secopsNavItems: NavItem[] = user?.role === "cybersecurity" ? [
    { id: "secops-insights", icon: ShieldAlert, label: "Threat Intel" },
    { id: "secops-utilities", icon: Wrench, label: "Security Tools" },
    { id: "secops-agents", icon: Bot, label: "AI Agents" },
    { id: "secops-scripts", icon: Zap, label: "Scripts" },
  ] : [];
  const marketerNavItems: NavItem[] = user?.role === "marketer" ? [
    { id: "marketer-insights", icon: DollarSign, label: "Analytics" },
    { id: "marketer-utilities", icon: Wrench, label: "Marketing Tools" },
    { id: "marketer-agents", icon: FlaskConical, label: "AI Agents" },
    { id: "marketer-scripts", icon: Zap, label: "Automation" },
  ] : [];
  const leaderNavItems: NavItem[] = user?.role === "team-leader" ? [
    { id: "leader-insights", icon: BarChart3, label: "Team Analytics" },
    { id: "leader-utilities", icon: Wrench, label: "PM Tools" },
    { id: "leader-agents", icon: Bot, label: "AI Agents" },
    { id: "leader-scripts", icon: Zap, label: "Automation" },
    { id: "leader-modals", icon: Layers, label: "Modals" },
  ] : [];
  const bottomNavItems: NavItem[] = [
    { id: "notifications", icon: Bell, label: "Notifications" },
    { id: "settings", icon: Settings2, label: "Settings" },
    { id: "profile", icon: User, label: "Profile" },
  ];
  const initials = user?.fullName?.split(" ").map(n => n[0]).join("").slice(0, 2).toUpperCase() || "U";
  const isAlwaysDark = ["developer", "cybersecurity", "marketer", "team-leader"].includes(currentRole || "");

  return (
    <div className="flex flex-col h-screen overflow-hidden bg-background">
      {/* ═══════ ZONE 1: TOP COMMAND CENTER ═══════ */}
      <motion.header
        animate={{ height: zenMode ? 0 : 56, opacity: zenMode ? 0 : 1 }}
        transition={{ duration: 0.3 }}
        className="border-b border-border/60 bg-card/50 backdrop-blur-2xl flex items-center justify-between px-5 shrink-0 z-30 overflow-hidden"
      >
        {/* Left: Logo + Workspace */}
        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2.5">
            <div className="w-8 h-8 rounded-xl bg-gradient-to-br from-primary to-primary/60 flex items-center justify-center shadow-lg shadow-primary/20 ring-1 ring-primary/20">
              <span className="text-primary-foreground font-bold text-sm">A</span>
            </div>
            {!sidebarCollapsed && <span className="text-sm font-bold text-foreground hidden sm:block tracking-tight">Atlas</span>}
          </div>
          <div className="h-5 w-px bg-border/40" />
          {/* Workspace Switcher */}
          <div className="relative">
            <button onClick={() => setWsDropdownOpen(!wsDropdownOpen)} className="flex items-center gap-2.5 px-3 py-2 rounded-xl hover:bg-muted/40 transition-all text-sm group">
              <div className="w-2 h-2 rounded-full bg-emerald-500 ring-2 ring-emerald-500/20" />
              <span className="font-medium text-foreground max-w-[180px] truncate">{activeWorkspace?.name || "No Workspace"}</span>
              <ChevronDown className={`w-3.5 h-3.5 text-muted-foreground transition-transform ${wsDropdownOpen ? "rotate-180" : ""}`} />
            </button>
            <AnimatePresence>
              {wsDropdownOpen && (
                <>
                  <div className="fixed inset-0 z-40" onClick={() => setWsDropdownOpen(false)} />
                  <motion.div initial={{ opacity: 0, y: -6, scale: 0.96 }} animate={{ opacity: 1, y: 0, scale: 1 }} exit={{ opacity: 0, y: -6, scale: 0.96 }} transition={{ duration: 0.15 }}
                    className="absolute top-full left-0 mt-2 w-72 bg-card border border-border rounded-2xl shadow-2xl shadow-black/20 overflow-hidden z-50">
                    <div className="p-3 border-b border-border">
                      <p className="text-[10px] font-bold text-muted-foreground uppercase tracking-widest px-1">Workspaces</p>
                    </div>
                    <div className="p-1.5 max-h-60 overflow-y-auto">
                      {workspaces.map((ws) => (
                        <button key={ws.id} onClick={() => { switchWorkspace(ws); setWsDropdownOpen(false); }}
                          className={`w-full flex items-center gap-2.5 px-3 py-2.5 rounded-xl text-sm transition-all ${ws.id === activeWorkspace?.id ? "bg-primary/8 text-primary font-medium" : "text-foreground hover:bg-muted/40"}`}>
                          {ws.isDefault ? <Star className="w-3.5 h-3.5 text-amber-500 fill-amber-500 shrink-0" /> : <div className="w-3.5 h-3.5 rounded border border-border/60 shrink-0" />}
                          <span className="truncate flex-1 text-left">{ws.name}</span>
                          {ws.activeIntegrations && ws.activeIntegrations.length > 0 && <span className="text-[10px] bg-muted px-2 py-0.5 rounded-full text-muted-foreground">{ws.activeIntegrations.length}</span>}
                          {ws.isDefault && <span className="text-[9px] bg-primary/10 text-primary px-2 py-0.5 rounded-full font-semibold">Default</span>}
                        </button>
                      ))}
                    </div>
                    <div className="p-1.5 border-t border-border">
                      <button onClick={() => { setCreateDialogOpen(true); setWsDropdownOpen(false); }} className="w-full flex items-center gap-2.5 px-3 py-2.5 rounded-xl text-sm text-primary hover:bg-primary/5 transition-colors font-medium">
                        <Plus className="w-4 h-4" /> New Workspace
                      </button>
                    </div>
                  </motion.div>
                </>
              )}
            </AnimatePresence>
          </div>
        </div>

        {/* Center: Command Palette Trigger */}
        <button onClick={() => setCommandOpen(true)}
          className="hidden md:flex items-center gap-3 px-4 py-2 rounded-xl bg-muted/20 border border-border/30 hover:border-primary/15 hover:bg-muted/30 transition-all max-w-lg w-full mx-8 group">
          <Search className="w-4 h-4 text-muted-foreground group-hover:text-primary transition-colors" />
          <span className="text-muted-foreground/60 text-xs flex-1 text-left">Search or run command...</span>
          <kbd className="hidden sm:flex items-center gap-0.5 text-[10px] text-muted-foreground/40 bg-muted/40 px-2 py-1 rounded-md border border-border/30">
            <Command className="w-3 h-3" />K
          </kbd>
        </button>

        {/* Right: Actions */}
        <div className="flex items-center gap-1.5">
          {/* Integration Status Dots */}
          <div className="hidden lg:flex items-center gap-1 mr-2">
            {integrations.slice(0, 4).map((int) => {
              const ProviderSvg = getProviderIcon(int.provider);
              return (
                <div key={int.id} className="w-7 h-7 rounded-lg flex items-center justify-center bg-muted/20 relative group hover:bg-muted/40 transition-colors" title={`${int.name}: ${int.status}`}>
                  {ProviderSvg ? <ProviderSvg size={14} /> : <Plug className="w-3.5 h-3.5 text-muted-foreground" />}
                  <div className={`absolute -bottom-0.5 -right-0.5 w-2 h-2 rounded-full ${statusColors[int.status] || "bg-zinc-400"} ring-2 ring-card`} />
                </div>
              );
            })}
          </div>

          {/* Zen Toggle */}
          <motion.button whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }} onClick={() => setZenMode(!zenMode)}
            className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium transition-all ${zenMode ? "bg-primary/10 text-primary border border-primary/20" : "text-muted-foreground hover:text-foreground hover:bg-muted/30 border border-transparent"}`}>
            {zenMode ? <EyeOff className="w-3.5 h-3.5" /> : <Eye className="w-3.5 h-3.5" />}
            <span className="hidden sm:inline">{zenMode ? "Exit Zen" : "Zen"}</span>
          </motion.button>

          <motion.button whileTap={isAlwaysDark ? {} : { rotate: 180 }} onClick={toggleTheme}
            className={`w-8 h-8 rounded-lg flex items-center justify-center transition-colors ${isAlwaysDark ? "text-muted-foreground/20 cursor-not-allowed" : "text-muted-foreground hover:bg-muted/40 hover:text-primary"}`}>
            {theme === "light" && !isAlwaysDark ? <Moon className="w-4 h-4" /> : <Sun className="w-4 h-4" />}
          </motion.button>

          <button onClick={() => setActiveTab("notifications")} className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted/40 hover:text-foreground transition-colors relative">
            <Bell className="w-4 h-4" />
            {unreadCount > 0 && (
              <span className="absolute -top-0.5 -right-0.5 min-w-[16px] h-4 flex items-center justify-center bg-red-500 rounded-full ring-2 ring-card text-[9px] font-bold text-white px-1">
                {unreadCount > 99 ? "99+" : unreadCount}
              </span>
            )}
          </button>

          {/* User Avatar Menu */}
          <div className="relative ml-1">
            <motion.button whileHover={{ scale: 1.05 }} onClick={() => setUserMenuOpen(!userMenuOpen)}
              className="w-8 h-8 rounded-full bg-gradient-to-br from-primary/20 to-primary/5 border border-primary/15 flex items-center justify-center ring-2 ring-primary/10 hover:ring-primary/20 transition-all">
              <span className="text-[10px] font-bold text-primary">{initials}</span>
            </motion.button>
            <AnimatePresence>
              {userMenuOpen && (
                <>
                  <div className="fixed inset-0 z-40" onClick={() => setUserMenuOpen(false)} />
                  <motion.div initial={{ opacity: 0, y: -6, scale: 0.95 }} animate={{ opacity: 1, y: 0, scale: 1 }} exit={{ opacity: 0, y: -6, scale: 0.95 }}
                    className="absolute top-full right-0 mt-2 w-56 bg-card border border-border rounded-2xl shadow-2xl shadow-black/20 z-50 overflow-hidden">
                    <div className="p-4 border-b border-border">
                      <p className="text-sm font-semibold text-foreground truncate">{user?.fullName}</p>
                      <p className="text-xs text-muted-foreground truncate mt-0.5">{user?.email}</p>
                      {user?.role && <span className="inline-flex mt-2 text-[10px] font-bold text-primary bg-primary/10 px-2 py-0.5 rounded-full uppercase tracking-wider">{user.role.replace("-", " ")}</span>}
                    </div>
                    <div className="p-1.5">
                      <button onClick={() => { setActiveTab("profile"); setUserMenuOpen(false); }} className="w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm text-foreground hover:bg-muted/50 transition-colors">
                        <User className="w-4 h-4 text-muted-foreground" /> Profile & Settings
                      </button>
                      <div className="my-1 mx-3 h-px bg-border" />
                      <button onClick={handleLogout} className="w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm text-red-500 hover:bg-red-500/8 transition-colors">
                        <LogOut className="w-4 h-4" /> Sign out
                      </button>
                    </div>
                  </motion.div>
                </>
              )}
            </AnimatePresence>
          </div>
        </div>
      </motion.header>

      {/* ═══════ MAIN CONTENT ═══════ */}
      <div className="flex flex-1 overflow-hidden">
        {/* ═══════ ZONE 2: LEFT SIDEBAR ═══════ */}
        <motion.aside
          animate={{ width: zenMode ? 0 : sidebarCollapsed ? 56 : 250, opacity: zenMode ? 0 : 1 }}
          transition={{ duration: 0.25, ease: "easeInOut" }}
          className="shrink-0 border-r border-border/40 bg-sidebar-background/40 backdrop-blur-xl flex flex-col h-full overflow-hidden relative"
        >
          {!zenMode && (
            <>
              <button onClick={() => setSidebarCollapsed(!sidebarCollapsed)}
                className="absolute -right-3.5 top-4 z-10 w-6 h-6 rounded-full bg-card border border-border flex items-center justify-center text-muted-foreground hover:text-foreground hover:bg-muted transition-all shadow-sm">
                {sidebarCollapsed ? <ChevronRight className="w-3 h-3" /> : <ChevronLeft className="w-3 h-3" />}
              </button>

              <div className={`${sidebarCollapsed ? "p-1.5" : "p-3"} flex-1 overflow-y-auto scrollbar-thin`}>
                <nav className="space-y-0.5 mb-4">
                  {coreNavItems.map((item, i) => (
                    <SidebarItem key={item.id} item={item} active={activeTab === item.id} collapsed={sidebarCollapsed} onClick={() => setActiveTab(item.id)} delay={i * 0.02} />
                  ))}
                </nav>

                {devNavItems.length > 0 && (
                  <>
                    {!sidebarCollapsed ? (
                      <div className="px-3 pb-1.5 pt-2">
                        <div className="h-px bg-border/40 mb-2" />
                        <p className="text-[10px] font-bold text-muted-foreground/50 tracking-[0.15em] uppercase">Developer</p>
                      </div>
                    ) : <div className="mx-2 my-2 h-px bg-border/40" />}
                    <nav className="space-y-0.5 mb-4">
                      {devNavItems.map((item, i) => (
                        <SidebarItem key={item.id} item={item} active={activeTab === item.id} collapsed={sidebarCollapsed} onClick={() => setActiveTab(item.id)} delay={0.08 + i * 0.02} />
                      ))}
                    </nav>
                  </>
                )}

                {designerNavItems.length > 0 && (
                  <>
                    {!sidebarCollapsed ? (
                      <div className="px-3 pb-1.5 pt-2">
                        <div className="h-px bg-border/40 mb-2" />
                        <p className="text-[10px] font-bold text-muted-foreground/50 tracking-[0.15em] uppercase">Designer</p>
                      </div>
                    ) : <div className="mx-2 my-2 h-px bg-border/40" />}
                    <nav className="space-y-0.5 mb-4">
                      {designerNavItems.map((item, i) => (
                        <SidebarItem key={item.id} item={item} active={activeTab === item.id} collapsed={sidebarCollapsed} onClick={() => setActiveTab(item.id)} delay={0.08 + i * 0.02} />
                      ))}
                    </nav>
                  </>
                )}

                {secopsNavItems.length > 0 && (
                  <>
                    {!sidebarCollapsed ? (
                      <div className="px-3 pb-1.5 pt-2">
                        <div className="h-px bg-border/40 mb-2" />
                        <p className="text-[10px] font-bold text-muted-foreground/50 tracking-[0.15em] uppercase">SecOps</p>
                      </div>
                    ) : <div className="mx-2 my-2 h-px bg-border/40" />}
                    <nav className="space-y-0.5 mb-4">
                      {secopsNavItems.map((item, i) => (
                        <SidebarItem key={item.id} item={item} active={activeTab === item.id} collapsed={sidebarCollapsed} onClick={() => setActiveTab(item.id)} delay={0.08 + i * 0.02} />
                      ))}
                    </nav>
                  </>
                )}

                {marketerNavItems.length > 0 && (
                  <>
                    {!sidebarCollapsed ? (
                      <div className="px-3 pb-1.5 pt-2">
                        <div className="h-px bg-border/40 mb-2" />
                        <p className="text-[10px] font-bold text-muted-foreground/50 tracking-[0.15em] uppercase">Marketing</p>
                      </div>
                    ) : <div className="mx-2 my-2 h-px bg-border/40" />}
                    <nav className="space-y-0.5 mb-4">
                      {marketerNavItems.map((item, i) => (
                        <SidebarItem key={item.id} item={item} active={activeTab === item.id} collapsed={sidebarCollapsed} onClick={() => setActiveTab(item.id)} delay={0.08 + i * 0.02} />
                      ))}
                    </nav>
                  </>
                )}

                {leaderNavItems.length > 0 && (
                  <>
                    {!sidebarCollapsed ? (
                      <div className="px-3 pb-1.5 pt-2">
                        <div className="h-px bg-border/40 mb-2" />
                        <p className="text-[10px] font-bold text-muted-foreground/50 tracking-[0.15em] uppercase">Leadership</p>
                      </div>
                    ) : <div className="mx-2 my-2 h-px bg-border/40" />}
                    <nav className="space-y-0.5 mb-4">
                      {leaderNavItems.map((item, i) => (
                        <SidebarItem key={item.id} item={item} active={activeTab === item.id} collapsed={sidebarCollapsed} onClick={() => setActiveTab(item.id)} delay={0.08 + i * 0.02} />
                      ))}
                    </nav>
                  </>
                )}

                {!sidebarCollapsed && workspaces.length > 0 && (
                  <div className="mb-4">
                    <div className="flex items-center justify-between px-3 mb-1.5">
                      <p className="text-[10px] font-bold text-muted-foreground/50 tracking-[0.15em] uppercase">Workspaces</p>
                      <button onClick={() => setCreateDialogOpen(true)} className="w-5 h-5 rounded-md flex items-center justify-center text-muted-foreground/30 hover:text-primary hover:bg-primary/5 transition-colors">
                        <Plus className="w-3 h-3" />
                      </button>
                    </div>
                    {workspaces.slice(0, 4).map((ws) => (
                      <button key={ws.id} onClick={() => switchWorkspace(ws)}
                        className={`w-full flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs transition-all ${ws.id === activeWorkspace?.id ? "bg-primary/8 text-primary font-medium" : "text-muted-foreground hover:bg-muted/30 hover:text-foreground"}`}>
                        {ws.isDefault ? <Star className="w-3 h-3 text-amber-500 fill-amber-500 shrink-0" /> : <div className="w-3 h-3 rounded-sm border border-border/50 shrink-0" />}
                        <span className="truncate flex-1 text-left">{ws.name}</span>
                      </button>
                    ))}
                  </div>
                )}

                {!sidebarCollapsed && allIntegrations.length > 0 && (
                  <div className="mb-4">
                    <p className="text-[10px] font-bold text-muted-foreground/50 tracking-[0.15em] uppercase px-3 mb-1.5">Integrations</p>
                    {allIntegrations.slice(0, 4).map((int) => {
                      const ProviderSvg = getProviderIcon(int.provider);
                      return (
                        <button key={int.id} onClick={() => setActiveTab("integrations")} className="w-full flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs text-muted-foreground hover:bg-muted/30 hover:text-foreground transition-all">
                          <span className="shrink-0 w-4 h-4 flex items-center justify-center">
                            {ProviderSvg ? <ProviderSvg size={13} /> : <Plug className="w-3 h-3" />}
                          </span>
                          <span className="truncate flex-1 text-left">{int.name}</span>
                          <div className={`w-1.5 h-1.5 rounded-full shrink-0 ${statusColors[int.status] || "bg-zinc-400"}`} />
                        </button>
                      );
                    })}
                  </div>
                )}

                <nav className="space-y-0.5">
                  {bottomNavItems.map((item) => (
                    <SidebarItem key={item.id} item={item} active={activeTab === item.id} collapsed={sidebarCollapsed} onClick={() => setActiveTab(item.id)} />
                  ))}
                </nav>
              </div>

              <div className={`${sidebarCollapsed ? "p-1.5" : "p-3"} border-t border-border/30`}>
                <motion.button whileHover={{ scale: 1.01 }} whileTap={{ scale: 0.99 }}
                  className="w-full flex items-center justify-center gap-2 h-9 rounded-xl bg-gradient-to-r from-primary to-primary/70 text-primary-foreground text-xs font-semibold shadow-lg shadow-primary/20 hover:shadow-primary/30 transition-shadow">
                  <Crown className="w-3.5 h-3.5" />
                  {!sidebarCollapsed && "Upgrade to Pro"}
                </motion.button>
              </div>
            </>
          )}
        </motion.aside>

        {/* ═══════ ZONE 3: CENTER STAGE ═══════ */}
        <main className="flex-1 overflow-y-auto scrollbar-thin">
          <div className={`mx-auto ${rightPanelOpen && !zenMode ? "max-w-5xl" : "max-w-6xl"} p-6 lg:p-8 transition-all duration-300`}>
            <AnimatePresence mode="wait">
              <motion.div key={activeTab} initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -8 }} transition={{ duration: 0.2, ease: "easeOut" }}>
                {activeTab === "overview" && <OverviewPanel workspaces={workspaces} integrations={integrations} pendingIntegrations={pendingIntegrations} activeWorkspace={activeWorkspace} onTabChange={setActiveTab} onCreateWorkspace={() => setCreateDialogOpen(true)} />}
                {activeTab === "workspaces" && <WorkspacesPanel workspaces={workspaces} activeWorkspace={activeWorkspace} integrations={integrations} onSwitchWorkspace={switchWorkspace} onCreateWorkspace={() => setCreateDialogOpen(true)} onDeleteWorkspace={deleteWorkspace} onSetDefault={setDefaultWorkspace} onToggleIntegration={toggleIntegration} onEnterWorkspace={(ws) => setEnteredWorkspace(ws)} />}
                {activeTab === "integrations" && <IntegrationsPanel integrations={integrations} pendingIntegrations={pendingIntegrations} activeWorkspace={activeWorkspace} workspaces={workspaces} onRefresh={refresh} />}
                {activeTab === "dev-overview" && <DevOverviewPanel onTabChange={setActiveTab} />}
                {activeTab === "insights" && <InsightsPanel />}
                {activeTab === "utilities" && <UtilitiesPanel />}
                {activeTab === "ai-agents" && <AIAgentsPanel />}
                {activeTab === "scripts" && <ScriptsPanel />}
                {activeTab === "snippets" && <SnippetsPanel />}
                {activeTab === "focus" && <FocusPanel />}
                {activeTab === "docker" && <DockerPanel />}
                {activeTab === "github" && <GitHubPanel integrations={integrations} />}
                {activeTab === "jira" && <JiraPanel integrations={integrations} />}
                {activeTab === "monitoring" && <MonitoringPanel integrations={integrations} />}
                {/* ─── Designer Panels ─── */}
                {activeTab === "designer-overview" && <DesignerOverviewPanel onTabChange={setActiveTab} />}
                {activeTab === "design-insights" && <DesignInsightsPanel />}
                {activeTab === "design-utilities" && <DesignUtilitiesPanel />}
                {activeTab === "figma" && <FigmaPanel integrations={integrations} />}
                {activeTab === "miro" && <MiroPanel integrations={integrations} />}
                {activeTab === "lottie" && <LottiePanel integrations={integrations} />}
                {activeTab === "dribbble" && <DribbblePanel integrations={integrations} />}
                {activeTab === "zeplin" && <ZeplinPanel integrations={integrations} />}
                {/* ─── SecOps Panels ─── */}
                {activeTab === "secops-overview" && <SecOpsOverviewPanel onTabChange={setActiveTab} />}
                {activeTab === "secops-insights" && <SecOpsInsightsPanel />}
                {activeTab === "secops-utilities" && <SecOpsUtilitiesPanel />}
                {activeTab === "secops-agents" && <SecOpsAgentsPanel />}
                {activeTab === "secops-scripts" && <SecOpsScriptsPanel />}
                {/* ─── Marketer Panels ─── */}
                {activeTab === "marketer-overview" && <MarketerOverviewPanel onTabChange={setActiveTab} />}
                {activeTab === "marketer-insights" && <MarketerInsightsPanel />}
                {activeTab === "marketer-utilities" && <MarketerUtilitiesPanel />}
                {activeTab === "marketer-agents" && <MarketerAgentsPanel />}
                {activeTab === "marketer-scripts" && <MarketerScriptsPanel />}
                {/* ─── Leader Panels ─── */}
                {activeTab === "leader-overview" && <LeaderOverviewPanel onTabChange={setActiveTab} />}
                {activeTab === "leader-insights" && <LeaderInsightsPanel />}
                {activeTab === "leader-utilities" && <LeaderUtilitiesPanel />}
                {activeTab === "leader-agents" && <LeaderAgentsPanel />}
                {activeTab === "leader-scripts" && <LeaderScriptsPanel />}
                {activeTab === "leader-modals" && <LeaderModalsPanel />}
                {/* ─── Shared Panels ─── */}
                {activeTab === "teams" && <TeamsPanel />}
                {activeTab === "omni-feed" && <OmniFeedPanel />}
                {activeTab === "squad-radar" && <SquadRadarPanel />}
                {activeTab === "squad-arena" && <SquadArenaPanel />}
                {activeTab === "resource-hub" && <ResourceHubPanel />}
                {activeTab === "knowledge" && <KnowledgePanel />}
                {activeTab === "projects" && <ProjectsPanel />}
                {activeTab === "notifications" && <NotificationsPanel />}
                {activeTab === "settings" && <SettingsPanel />}
                {activeTab === "profile" && <ProfilePanel />}
              </motion.div>
            </AnimatePresence>
          </div>
        </main>

        {/* ═══════ ZONE 4: RIGHT PULSE PANEL ═══════ */}
        <AnimatePresence>
          {rightPanelOpen && !zenMode && (
            <motion.aside initial={{ width: 0, opacity: 0 }} animate={{ width: 280, opacity: 1 }} exit={{ width: 0, opacity: 0 }} transition={{ duration: 0.25 }}
              className="shrink-0 border-l border-border/40 bg-sidebar-background/20 backdrop-blur-xl overflow-hidden">
              <RightPulsePanel integrations={integrations} activeWorkspace={activeWorkspace} onClose={() => setRightPanelOpen(false)} />
            </motion.aside>
          )}
        </AnimatePresence>

        {!rightPanelOpen && !zenMode && (
          <button onClick={() => setRightPanelOpen(true)}
            className="fixed right-0 top-1/2 -translate-y-1/2 z-20 w-6 h-12 rounded-l-lg bg-card border border-border border-r-0 flex items-center justify-center text-muted-foreground hover:text-primary transition-colors shadow-lg">
            <PanelRightOpen className="w-3.5 h-3.5" />
          </button>
        )}
      </div>

      {/* ═══════ ZONE 5: BOTTOM AI TERMINAL (Zen mode) ═══════ */}
      <AnimatePresence>
        {zenMode && (
          <motion.div initial={{ y: 48, opacity: 0 }} animate={{ y: 0, opacity: 1 }} exit={{ y: 48, opacity: 0 }}
            className="h-12 border-t border-border/40 bg-card/60 backdrop-blur-2xl flex items-center px-5 shrink-0 gap-3">
            <Bot className="w-4 h-4 text-primary" />
            <input type="text" placeholder="Ask Atlas AI... (ESC to exit Zen mode)" className="flex-1 bg-transparent text-sm text-foreground placeholder:text-muted-foreground/40 outline-none" />
            <button onClick={() => setZenMode(false)} className="text-xs text-muted-foreground hover:text-foreground bg-muted/40 px-2 py-1 rounded-md transition-colors">ESC</button>
          </motion.div>
        )}
      </AnimatePresence>

      {/* ═══════ COMMAND PALETTE (Real API Search) ═══════ */}
      <CommandPalette open={commandOpen} onClose={() => setCommandOpen(false)} onNavigate={(route) => { setActiveTab(route); setCommandOpen(false); }} />

      <CreateWorkspaceDialog open={createDialogOpen} onClose={() => setCreateDialogOpen(false)} onCreate={createWorkspace} />
      <AnimatePresence>
        {enteredWorkspace && <WorkspaceImmersiveView workspace={enteredWorkspace} integrations={integrations} onExit={() => setEnteredWorkspace(null)} />}
      </AnimatePresence>
    </div>
  );
};

// ═══════════════════════════════════════════════════════════════════
// SIDEBAR ITEM — Premium
// ═══════════════════════════════════════════════════════════════════
const SidebarItem = ({ item, active, collapsed, onClick, delay = 0 }: { item: NavItem; active: boolean; collapsed: boolean; onClick: () => void; delay?: number }) => (
  <motion.button
    initial={{ opacity: 0, x: -6 }}
    animate={{ opacity: 1, x: 0 }}
    transition={{ delay }}
    whileHover={{ x: collapsed ? 0 : 2 }}
    onClick={onClick}
    title={collapsed ? item.label : undefined}
    className={`w-full flex items-center gap-2.5 ${collapsed ? "justify-center px-0 py-2.5" : "px-3 py-2"} rounded-xl text-sm transition-all ${
      active
        ? "bg-primary/10 text-primary font-medium shadow-sm shadow-primary/5"
        : item.highlight
          ? "text-primary/60 hover:bg-primary/5 hover:text-primary"
          : "text-muted-foreground hover:bg-muted/30 hover:text-foreground"
    }`}
  >
    <item.icon className={`${collapsed ? "w-5 h-5" : "w-4 h-4"} shrink-0`} />
    {!collapsed && <span className="flex-1 text-left truncate">{item.label}</span>}
    {!collapsed && item.badge && <span className="text-[10px] bg-muted/50 text-muted-foreground px-1.5 py-0.5 rounded-md font-medium">{item.badge}</span>}
  </motion.button>
);

// ═══════════════════════════════════════════════════════════════════
// RIGHT PULSE PANEL — Premium
// ═══════════════════════════════════════════════════════════════════
const RightPulsePanel = ({ integrations, activeWorkspace, onClose }: { integrations: IntegrationDto[]; activeWorkspace: WorkspaceDto | null; onClose: () => void }) => (
  <div className="h-full flex flex-col p-4 overflow-y-auto scrollbar-thin">
    <div className="flex items-center justify-between mb-4">
      <div className="flex items-center gap-2">
        <div className="w-2 h-2 rounded-full bg-emerald-500 animate-pulse" />
        <h3 className="text-xs font-bold text-foreground tracking-tight uppercase">Live Pulse</h3>
      </div>
      <button onClick={onClose} className="w-6 h-6 rounded-lg flex items-center justify-center text-muted-foreground hover:text-foreground hover:bg-muted/40 transition-colors">
        <PanelRightClose className="w-3.5 h-3.5" />
      </button>
    </div>

    {activeWorkspace && (
      <div className="mb-4 p-3.5 rounded-2xl bg-gradient-to-br from-primary/5 to-transparent border border-primary/8">
        <div className="flex items-center gap-2 mb-2">
          <FolderOpen className="w-4 h-4 text-primary" />
          <p className="text-sm font-semibold text-foreground truncate">{activeWorkspace.name}</p>
        </div>
        {activeWorkspace.activeIntegrations && activeWorkspace.activeIntegrations.length > 0 ? (
          <div className="flex flex-wrap gap-1.5">
            {activeWorkspace.activeIntegrations.map((ai) => {
              const ProviderSvg = getProviderIcon(ai.provider);
              return (
                <div key={ai.integrationId} className="flex items-center gap-1.5 px-2 py-1 rounded-lg bg-card/40 border border-border/20 text-xs text-muted-foreground">
                  {ProviderSvg && <ProviderSvg size={12} />}
                  <span>{ai.integrationName}</span>
                  <div className={`w-1.5 h-1.5 rounded-full ${ai.enabled ? "bg-emerald-500" : "bg-zinc-400"}`} />
                </div>
              );
            })}
          </div>
        ) : <p className="text-xs text-muted-foreground/50">No integrations connected</p>}
      </div>
    )}

    <div className="mb-4">
      <div className="flex items-center gap-2 mb-2">
        <Radio className="w-3.5 h-3.5 text-emerald-500" />
        <p className="text-[10px] font-bold text-muted-foreground uppercase tracking-wider">Status</p>
      </div>
      <div className="space-y-1.5">
        {integrations.slice(0, 5).map((int) => {
          const ProviderSvg = getProviderIcon(int.provider);
          return (
            <div key={int.id} className="flex items-center gap-2 px-3 py-2 rounded-xl bg-card/30 border border-border/15 hover:bg-card/50 transition-colors">
              <span className="w-4 h-4 flex items-center justify-center shrink-0">{ProviderSvg ? <ProviderSvg size={13} /> : <Plug className="w-3 h-3 text-muted-foreground" />}</span>
              <span className="text-xs text-foreground flex-1 truncate">{int.name}</span>
              <div className={`w-2 h-2 rounded-full ${statusColors[int.status] || "bg-zinc-400"}`} />
            </div>
          );
        })}
      </div>
    </div>

    <div className="mb-4">
      <div className="flex items-center gap-2 mb-2">
        <Activity className="w-3.5 h-3.5 text-blue-400" />
        <p className="text-[10px] font-bold text-muted-foreground uppercase tracking-wider">Activity</p>
      </div>
      {[
        { text: "Workspace data synced", time: "just now", color: "bg-emerald-500" },
        { text: "Integrations loaded", time: "1m ago", color: "bg-blue-500" },
        { text: "Session active", time: "2m ago", color: "bg-amber-500" },
      ].map((feed, i) => (
        <div key={i} className="flex items-start gap-2.5 px-3 py-2 rounded-lg hover:bg-muted/15 transition-colors">
          <div className={`w-1.5 h-1.5 rounded-full ${feed.color} mt-1.5 shrink-0`} />
          <div>
            <p className="text-xs text-foreground leading-snug">{feed.text}</p>
            <p className="text-[10px] text-muted-foreground/50">{feed.time}</p>
          </div>
        </div>
      ))}
    </div>

    <div>
      <div className="flex items-center gap-2 mb-2">
        <Trophy className="w-3.5 h-3.5 text-amber-500" />
        <p className="text-[10px] font-bold text-muted-foreground uppercase tracking-wider">Bounties</p>
      </div>
      <div className="p-4 rounded-2xl bg-gradient-to-br from-amber-500/5 to-transparent border border-amber-500/8 text-center">
        <Trophy className="w-6 h-6 text-amber-500/20 mx-auto mb-2" />
        <p className="text-xs text-muted-foreground">No active bounties</p>
        <p className="text-[10px] text-muted-foreground/40 mt-1">Create a team to start</p>
      </div>
    </div>
  </div>
);


export default Dashboard;
