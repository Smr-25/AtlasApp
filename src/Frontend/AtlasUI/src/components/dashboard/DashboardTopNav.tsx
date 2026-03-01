import { Search, Settings, Bell, Sun, Moon, LogOut, ChevronDown, User } from "lucide-react";
import { useTheme } from "@/context/ThemeContext";
import { useAuth } from "@/context/AuthContext";
import { motion, AnimatePresence } from "framer-motion";
import { WorkspaceDto } from "@/services/api";
import { useState, useRef, useEffect } from "react";
import { useNavigate } from "react-router-dom";

interface DashboardTopNavProps {
  activeWorkspace: WorkspaceDto | null;
  workspaces: WorkspaceDto[];
  onSwitchWorkspace: (ws: WorkspaceDto) => void;
  onTabChange?: (tab: string) => void;
}

const DashboardTopNav = ({ activeWorkspace, workspaces, onSwitchWorkspace, onTabChange }: DashboardTopNavProps) => {
  const { theme, toggleTheme, currentRole, clearRole } = useTheme();
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [wsOpen, setWsOpen] = useState(false);
  const [userOpen, setUserOpen] = useState(false);
  const wsRef = useRef<HTMLDivElement>(null);
  const userRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (wsRef.current && !wsRef.current.contains(e.target as Node)) setWsOpen(false);
      if (userRef.current && !userRef.current.contains(e.target as Node)) setUserOpen(false);
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, []);

  const handleLogout = async () => {
    clearRole();
    await logout();
    navigate("/login");
  };

  const initials = user?.fullName
    ?.split(" ")
    .map((n) => n[0])
    .join("")
    .slice(0, 2)
    .toUpperCase() || "U";

  return (
    <motion.header
      initial={{ opacity: 0, y: -10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4 }}
      className="h-14 border-b border-border bg-card/80 backdrop-blur-xl flex items-center justify-between px-5 shrink-0 z-30"
    >
      {/* Left — Logo + Workspace Switcher */}
      <div className="flex items-center gap-4">
        <div className="flex items-center gap-2.5">
          <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-primary to-primary/70 flex items-center justify-center shadow-md shadow-primary/20">
            <span className="text-primary-foreground font-bold text-sm">A</span>
          </div>
          <span className="text-sm font-semibold text-foreground hidden sm:block">Atlas</span>
        </div>

        <div className="h-5 w-px bg-border" />

        {/* Workspace Switcher */}
        <div className="relative" ref={wsRef}>
          <button
            onClick={() => setWsOpen(!wsOpen)}
            className="flex items-center gap-2 px-3 py-1.5 rounded-lg hover:bg-muted/60 transition-colors text-sm"
          >
            <div className="w-2 h-2 rounded-full bg-primary animate-pulse" />
            <span className="font-medium text-foreground max-w-[140px] truncate">
              {activeWorkspace?.name || "No Workspace"}
            </span>
            <ChevronDown className={`w-3.5 h-3.5 text-muted-foreground transition-transform ${wsOpen ? "rotate-180" : ""}`} />
          </button>

          <AnimatePresence>
            {wsOpen && (
              <motion.div
                initial={{ opacity: 0, y: -5, scale: 0.95 }}
                animate={{ opacity: 1, y: 0, scale: 1 }}
                exit={{ opacity: 0, y: -5, scale: 0.95 }}
                className="absolute top-full left-0 mt-1.5 w-64 bg-card border border-border rounded-xl shadow-xl shadow-black/10 overflow-hidden z-50"
              >
                <div className="p-2 border-b border-border">
                  <p className="text-[10px] font-semibold text-muted-foreground uppercase tracking-wider px-2 py-1">Workspaces</p>
                </div>
                <div className="p-1.5 max-h-48 overflow-y-auto">
                  {workspaces.map((ws) => (
                    <button
                      key={ws.id}
                      onClick={() => { onSwitchWorkspace(ws); setWsOpen(false); }}
                      className={`w-full flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm transition-colors ${
                        ws.id === activeWorkspace?.id
                          ? "bg-primary/10 text-primary font-medium"
                          : "text-foreground hover:bg-muted"
                      }`}
                    >
                      <div className={`w-2 h-2 rounded-full ${ws.isDefault ? "bg-primary" : "bg-muted-foreground/30"}`} />
                      <span className="truncate flex-1 text-left">{ws.name}</span>
                      {ws.isDefault && <span className="text-[9px] bg-primary/10 text-primary px-1.5 py-0.5 rounded-md font-medium">Default</span>}
                    </button>
                  ))}
                </div>
              </motion.div>
            )}
          </AnimatePresence>
        </div>
      </div>

      {/* Center — Search */}
      <div className="hidden md:flex flex-1 max-w-md mx-6">
        <div className="relative w-full group">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-muted-foreground group-focus-within:text-primary transition-colors" />
          <input
            type="text"
            placeholder="Search workspaces, integrations..."
            className="w-full h-8 pl-9 pr-4 rounded-lg bg-muted/40 border border-transparent text-xs text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-1 focus:ring-primary/30 focus:border-primary/30 focus:bg-muted/60 transition-all"
          />
        </div>
      </div>

      {/* Right — Actions */}
      <div className="flex items-center gap-0.5">
        {(() => {
          const isAlwaysDark = ["developer", "cybersecurity", "marketer"].includes(currentRole);
          return (
            <motion.button
              whileTap={isAlwaysDark ? {} : { rotate: 180 }}
              onClick={toggleTheme}
              className={`w-8 h-8 rounded-lg flex items-center justify-center transition-colors ${
                isAlwaysDark
                  ? "text-muted-foreground/30 cursor-not-allowed"
                  : "text-muted-foreground hover:bg-muted hover:text-primary"
              }`}
              title={isAlwaysDark ? "Dark mode is native for your role" : "Toggle theme"}
            >
              {theme === "light" && !isAlwaysDark ? <Moon className="w-4 h-4" /> : <Sun className="w-4 h-4" />}
            </motion.button>
          );
        })()}

        <button className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-foreground transition-colors relative">
          <Bell className="w-4 h-4" />
          <span className="absolute top-1.5 right-1.5 w-1.5 h-1.5 bg-red-500 rounded-full" />
        </button>

        <button className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted hover:text-foreground transition-colors">
          <Settings className="w-4 h-4" />
        </button>

        {/* User Menu */}
        <div className="relative ml-1" ref={userRef}>
          <motion.button
            whileHover={{ scale: 1.05 }}
            onClick={() => setUserOpen(!userOpen)}
            className="w-8 h-8 rounded-full bg-gradient-to-br from-primary/20 to-primary/5 border border-primary/20 flex items-center justify-center"
          >
            <span className="text-[11px] font-semibold text-primary">{initials}</span>
          </motion.button>

          <AnimatePresence>
            {userOpen && (
              <motion.div
                initial={{ opacity: 0, y: -5, scale: 0.95 }}
                animate={{ opacity: 1, y: 0, scale: 1 }}
                exit={{ opacity: 0, y: -5, scale: 0.95 }}
                className="absolute top-full right-0 mt-1.5 w-52 bg-card border border-border rounded-xl shadow-xl shadow-black/10 overflow-hidden z-50"
              >
                <div className="p-3 border-b border-border">
                  <p className="text-sm font-medium text-foreground truncate">{user?.fullName}</p>
                  <p className="text-[11px] text-muted-foreground truncate">{user?.email}</p>
                </div>
                <div className="p-1.5">
                  <button
                    onClick={() => { onTabChange?.("profile"); setUserOpen(false); }}
                    className="w-full flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm text-foreground hover:bg-muted transition-colors"
                  >
                    <User className="w-4 h-4" />
                    Profile
                  </button>
                  <button
                    onClick={() => { onTabChange?.("profile"); setUserOpen(false); }}
                    className="w-full flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm text-foreground hover:bg-muted transition-colors"
                  >
                    <Settings className="w-4 h-4" />
                    Settings
                  </button>
                  <div className="my-1 h-px bg-border" />
                  <button
                    onClick={handleLogout}
                    className="w-full flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm text-red-500 hover:bg-red-500/10 transition-colors"
                  >
                    <LogOut className="w-4 h-4" />
                    Sign out
                  </button>
                </div>
              </motion.div>
            )}
          </AnimatePresence>
        </div>
      </div>
    </motion.header>
  );
};

export default DashboardTopNav;

