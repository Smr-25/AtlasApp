import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import {
  Plug,
  AlertCircle,
  RefreshCw,
  Trash2,
  Loader2,
  Check,
  Clock,
  XCircle,
  ExternalLink,
  Search,
  X,
  Key,
  Link2,
  Star,
} from "lucide-react";
import { IntegrationDto, integrationApi, WorkspaceDto, workspaceApi } from "@/services/api";
import { useAuth } from "@/context/AuthContext";
import {
  getProvidersForRole,
  getProviderInfo,
  IntegrationProviderInfo,
} from "@/lib/integration-providers";
import { getProviderIcon } from "@/components/icons/IntegrationIcons";

interface IntegrationsPanelProps {
  integrations: IntegrationDto[];
  pendingIntegrations: IntegrationDto[];
  activeWorkspace: WorkspaceDto | null;
  workspaces: WorkspaceDto[];
  onRefresh: () => Promise<void>;
}

const statusConfig: Record<string, { icon: typeof Check; color: string; bg: string; label: string }> = {
  Active: { icon: Check, color: "text-emerald-600", bg: "bg-emerald-500/10", label: "Active" },
  PendingSetup: { icon: Clock, color: "text-amber-600", bg: "bg-amber-500/10", label: "Needs Setup" },
  Expired: { icon: AlertCircle, color: "text-red-500", bg: "bg-red-500/10", label: "Expired" },
  Error: { icon: XCircle, color: "text-red-500", bg: "bg-red-500/10", label: "Error" },
  Disconnected: { icon: Plug, color: "text-zinc-500", bg: "bg-zinc-500/10", label: "Disconnected" },
};

const IntegrationsPanel = ({ integrations, pendingIntegrations, activeWorkspace, workspaces, onRefresh }: IntegrationsPanelProps) => {
  const { user } = useAuth();
  const [actionLoading, setActionLoading] = useState<string | null>(null);
  const [view, setView] = useState<"connected" | "catalog">("connected");
  const [catalogSearch, setCatalogSearch] = useState("");
  const [selectedCategory, setSelectedCategory] = useState<string>("all");

  // Reconnect dialog state
  const [reconnectTarget, setReconnectTarget] = useState<IntegrationDto | null>(null);
  const [reconnectToken, setReconnectToken] = useState("");
  const [reconnectLoading, setReconnectLoading] = useState(false);
  const [reconnectError, setReconnectError] = useState("");

  // Toggle to workspace dialog
  const [toggleTarget, setToggleTarget] = useState<IntegrationDto | null>(null);
  const [toggleWorkspaceId, setToggleWorkspaceId] = useState("");
  const [toggleLoading, setToggleLoading] = useState(false);

  // Connect new integration dialog
  const [connectTarget, setConnectTarget] = useState<IntegrationProviderInfo | null>(null);
  const [connectToken, setConnectToken] = useState("");
  const [connectLoading, setConnectLoading] = useState(false);
  const [connectError, setConnectError] = useState("");

  const allConnected = [...integrations, ...pendingIntegrations];
  const connectedProviders = new Set(allConnected.map((i) => i.provider));

  const roleProviders = getProvidersForRole(user?.role);
  const categories = ["all", ...new Set(roleProviders.map((p) => p.category))];
  const filteredCatalog = roleProviders.filter((p) => {
    const matchSearch = !catalogSearch || p.name.toLowerCase().includes(catalogSearch.toLowerCase()) || p.description.toLowerCase().includes(catalogSearch.toLowerCase());
    const matchCategory = selectedCategory === "all" || p.category === selectedCategory;
    return matchSearch && matchCategory;
  });

  const handleDelete = async (id: string) => {
    setActionLoading(id);
    try {
      await integrationApi.remove(id);
      await onRefresh();
    } finally {
      setActionLoading(null);
    }
  };

  const handleReconnect = async () => {
    if (!reconnectTarget || !reconnectToken.trim()) return;
    setReconnectLoading(true);
    setReconnectError("");
    try {
      await integrationApi.reconnect(reconnectTarget.id, {
        integrationId: reconnectTarget.id,
        accessToken: reconnectToken.trim(),
      });
      await onRefresh();
      setReconnectTarget(null);
      setReconnectToken("");
    } catch (err: any) {
      const msg = err?.response?.data?.errors?.[0] || "Failed to reconnect";
      setReconnectError(msg);
    } finally {
      setReconnectLoading(false);
    }
  };

  const handleToggleToWorkspace = async () => {
    if (!toggleTarget || !toggleWorkspaceId) return;
    setToggleLoading(true);
    try {
      await workspaceApi.toggleIntegration(toggleWorkspaceId, toggleTarget.id, true);
      await onRefresh();
      setToggleTarget(null);
      setToggleWorkspaceId("");
    } finally {
      setToggleLoading(false);
    }
  };

  const handleConnect = async () => {
    if (!connectTarget || !connectToken.trim()) return;
    setConnectLoading(true);
    setConnectError("");
    try {
      await integrationApi.create({
        provider: connectTarget.provider,
        name: connectTarget.name,
        accessToken: connectToken.trim(),
      });
      await onRefresh();
      setConnectTarget(null);
      setConnectToken("");
      setView("connected");
    } catch (err: any) {
      const msg = err?.response?.data?.errors?.[0] || "Failed to connect integration";
      setConnectError(msg);
    } finally {
      setConnectLoading(false);
    }
  };

  // Find which workspaces an integration is already bound to
  const getWorkspacesForIntegration = (intId: string) =>
    workspaces.filter((ws) => ws.activeIntegrations?.some((ai) => ai.integrationId === intId));

  const inputClass = "w-full h-10 px-3.5 rounded-lg bg-muted/40 border border-border text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary/40 transition-all";

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-violet-500/20 to-violet-500/5 border border-violet-500/10 flex items-center justify-center">
            <Plug className="w-5 h-5 text-violet-400" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-foreground tracking-tight">Integrations</h2>
            <p className="text-xs text-muted-foreground">
              {allConnected.length} connected · {roleProviders.length} available for your role
            </p>
          </div>
        </div>
      </div>

      {/* Tab Switcher */}
      <div className="flex gap-1 p-1 bg-muted/30 border border-border rounded-xl w-fit">
        <button
          onClick={() => setView("connected")}
          className={`px-4 py-1.5 rounded-md text-xs font-medium transition-all ${
            view === "connected" ? "bg-card text-foreground shadow-sm" : "text-muted-foreground hover:text-foreground"
          }`}
        >
          Connected
          <span className="ml-1.5 text-[10px] text-muted-foreground">({allConnected.length})</span>
        </button>
        <button
          onClick={() => setView("catalog")}
          className={`px-4 py-1.5 rounded-md text-xs font-medium transition-all ${
            view === "catalog" ? "bg-card text-foreground shadow-sm" : "text-muted-foreground hover:text-foreground"
          }`}
        >
          Available
          <span className="ml-1.5 text-[10px] text-muted-foreground">({roleProviders.length})</span>
        </button>
      </div>

      {/* Pending Alert */}
      {view === "connected" && pendingIntegrations.length > 0 && (
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          className="flex items-center gap-3 p-4 rounded-xl bg-amber-500/5 border border-amber-500/15"
        >
          <div className="w-9 h-9 rounded-lg bg-amber-500/10 flex items-center justify-center shrink-0">
            <AlertCircle className="w-5 h-5 text-amber-600" />
          </div>
          <div className="flex-1">
            <p className="text-sm font-medium text-foreground">
              {pendingIntegrations.length} integration{pendingIntegrations.length !== 1 ? "s" : ""} waiting for setup
            </p>
            <p className="text-[11px] text-muted-foreground">Provide your API tokens to activate these integrations</p>
          </div>
        </motion.div>
      )}

      {/* ─── Connected View ─── */}
      {view === "connected" && (
        <div className="space-y-2">
          {allConnected.length === 0 ? (
            <div className="py-16 text-center">
              <Plug className="w-12 h-12 text-muted-foreground/20 mx-auto mb-3" />
              <p className="text-sm font-medium text-foreground mb-1">No integrations connected</p>
              <p className="text-xs text-muted-foreground mb-4">Browse the catalog to connect your first tool</p>
              <button
                onClick={() => setView("catalog")}
                className="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-primary text-primary-foreground text-xs font-medium shadow-md shadow-primary/20"
              >
                Browse Catalog
              </button>
            </div>
          ) : (
            allConnected.map((int, i) => {
              const cfg = statusConfig[int.status] || statusConfig.Disconnected;
              const StatusIcon = cfg.icon;
              const providerInfo = getProviderInfo(int.provider);
              const ProviderSvg = getProviderIcon(int.provider);
              const boundWs = getWorkspacesForIntegration(int.id);
              return (
                <motion.div
                  key={int.id}
                  initial={{ opacity: 0, y: 10 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: i * 0.03 }}
                  className="relative overflow-hidden flex items-center gap-4 p-4 bg-gradient-to-br from-card to-card/60 rounded-xl border border-border hover:border-primary/15 transition-all group"
                >
                  <div className="absolute top-0 right-0 w-20 h-20 bg-primary/[0.015] rounded-full -translate-y-1/2 translate-x-1/3 group-hover:bg-primary/[0.03] transition-colors duration-500" />
                  <div className="w-10 h-10 rounded-lg bg-muted/40 border border-border/50 flex items-center justify-center shrink-0">
                    {ProviderSvg ? <ProviderSvg size={22} /> : <Plug className="w-5 h-5 text-muted-foreground" />}
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 flex-wrap">
                      <p className="text-sm font-medium text-foreground truncate">{int.name}</p>
                      <span className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-[10px] font-medium ${cfg.bg} ${cfg.color}`}>
                        <StatusIcon className="w-2.5 h-2.5" />
                        {cfg.label}
                      </span>
                    </div>
                    <p className="text-[11px] text-muted-foreground">{providerInfo?.description || int.provider}</p>
                    {boundWs.length > 0 && (
                      <div className="flex items-center gap-1 mt-1 flex-wrap">
                        <Link2 className="w-3 h-3 text-muted-foreground/50" />
                        {boundWs.map((ws) => (
                          <span key={ws.id} className="text-[9px] bg-muted/60 text-muted-foreground px-1.5 py-0.5 rounded">{ws.name}</span>
                        ))}
                      </div>
                    )}
                  </div>
                  <div className="flex items-center gap-1 shrink-0">
                    {/* Activate PendingSetup */}
                    {int.status === "PendingSetup" && (
                      <button
                        onClick={() => { setReconnectTarget(int); setReconnectToken(""); setReconnectError(""); }}
                        className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium text-amber-700 bg-amber-500/10 hover:bg-amber-500/20 transition-colors"
                      >
                        <Key className="w-3.5 h-3.5" /> Setup
                      </button>
                    )}
                    {/* Reconnect expired */}
                    {(int.status === "Expired" || int.status === "Error") && (
                      <button
                        onClick={() => { setReconnectTarget(int); setReconnectToken(""); setReconnectError(""); }}
                        className="w-8 h-8 rounded-lg flex items-center justify-center text-primary hover:bg-primary/10 transition-colors"
                        title="Reconnect"
                      >
                        <RefreshCw className="w-4 h-4" />
                      </button>
                    )}
                    {/* Bind to workspace */}
                    {int.status === "Active" && (
                      <button
                        onClick={() => { setToggleTarget(int); setToggleWorkspaceId(activeWorkspace?.id || ""); }}
                        className="w-8 h-8 rounded-lg flex items-center justify-center text-primary hover:bg-primary/10 transition-colors"
                        title="Link to workspace"
                      >
                        <Link2 className="w-4 h-4" />
                      </button>
                    )}
                    {/* Delete */}
                    <button
                      onClick={() => handleDelete(int.id)}
                      disabled={actionLoading === int.id}
                      className="w-8 h-8 rounded-lg flex items-center justify-center text-red-500 hover:bg-red-500/10 transition-colors opacity-0 group-hover:opacity-100"
                      title="Disconnect"
                    >
                      {actionLoading === int.id ? <Loader2 className="w-4 h-4 animate-spin" /> : <Trash2 className="w-4 h-4" />}
                    </button>
                  </div>
                </motion.div>
              );
            })
          )}
        </div>
      )}

      {/* ─── Catalog View ─── */}
      {view === "catalog" && (
        <div className="space-y-4">
          <div className="flex flex-col sm:flex-row gap-3">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-muted-foreground" />
              <input
                type="text"
                placeholder="Search integrations..."
                value={catalogSearch}
                onChange={(e) => setCatalogSearch(e.target.value)}
                className="w-full h-9 pl-9 pr-4 rounded-lg bg-muted/40 border border-border text-xs text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-1 focus:ring-primary/30 transition-all"
              />
            </div>
            <div className="flex gap-1 flex-wrap">
              {categories.map((cat) => (
                <button
                  key={cat}
                  onClick={() => setSelectedCategory(cat)}
                  className={`px-3 py-1.5 rounded-md text-[11px] font-medium transition-all capitalize ${
                    selectedCategory === cat ? "bg-primary/10 text-primary" : "text-muted-foreground hover:text-foreground hover:bg-muted"
                  }`}
                >
                  {cat}
                </button>
              ))}
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-3">
            {filteredCatalog.map((p, i) => {
              const isConnected = connectedProviders.has(p.provider);
              const ProviderSvg = getProviderIcon(p.provider);
              return (
                <motion.div
                  key={p.provider}
                  initial={{ opacity: 0, y: 10 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: i * 0.02 }}
                  whileHover={{ y: -2 }}
                  className={`relative p-4 rounded-xl border transition-all ${
                    isConnected ? "bg-primary/5 border-primary/20" : "bg-card border-border hover:border-primary/15"
                  }`}
                >
                  <div className="flex items-start gap-3">
                    <div className="w-10 h-10 rounded-lg bg-muted/50 flex items-center justify-center shrink-0">
                      {ProviderSvg ? <ProviderSvg size={22} /> : <Plug className="w-5 h-5 text-muted-foreground" />}
                    </div>
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2">
                        <h4 className="text-sm font-medium text-foreground">{p.name}</h4>
                        {isConnected && (
                          <span className="text-[9px] font-medium text-emerald-600 bg-emerald-500/10 px-1.5 py-0.5 rounded-full flex items-center gap-0.5">
                            <Check className="w-2.5 h-2.5" /> Connected
                          </span>
                        )}
                      </div>
                      <p className="text-[11px] text-muted-foreground mt-0.5 line-clamp-2">{p.description}</p>
                      <div className="flex items-center gap-2 mt-2">
                        <span className="text-[9px] text-muted-foreground/70 bg-muted/50 px-1.5 py-0.5 rounded">{p.category}</span>
                        {p.apiUrl && <ExternalLink className="w-3 h-3 text-muted-foreground/40" />}
                      </div>
                      {/* Connect button */}
                      {!isConnected && (
                        <motion.button
                          whileTap={{ scale: 0.97 }}
                          onClick={() => { setConnectTarget(p); setConnectToken(""); setConnectError(""); }}
                          className="mt-3 w-full flex items-center justify-center gap-1.5 h-8 rounded-lg bg-primary text-primary-foreground text-xs font-medium shadow-sm shadow-primary/15 hover:shadow-primary/30 transition-all"
                        >
                          <Plug className="w-3.5 h-3.5" />
                          Connect
                        </motion.button>
                      )}
                    </div>
                  </div>
                </motion.div>
              );
            })}
          </div>

          {filteredCatalog.length === 0 && (
            <div className="py-12 text-center">
              <Search className="w-10 h-10 text-muted-foreground/20 mx-auto mb-3" />
              <p className="text-sm text-muted-foreground">No integrations found matching your search</p>
            </div>
          )}
        </div>
      )}

      {/* ─── Reconnect / Setup Dialog ─── */}
      <AnimatePresence>
        {reconnectTarget && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 z-50 flex items-center justify-center">
            <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={() => setReconnectTarget(null)} />
            <motion.div
              initial={{ opacity: 0, scale: 0.95, y: 10 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.95, y: 10 }}
              className="relative w-full max-w-md bg-card border border-border rounded-2xl shadow-2xl overflow-hidden"
            >
              <div className="flex items-center justify-between p-5 border-b border-border">
                <div className="flex items-center gap-3">
                  <div className="w-9 h-9 rounded-lg bg-primary/10 flex items-center justify-center">
                    {(() => { const Svg = getProviderIcon(reconnectTarget.provider); return Svg ? <Svg size={20} /> : <Key className="w-5 h-5 text-primary" />; })()}
                  </div>
                  <div>
                    <h3 className="text-sm font-semibold text-foreground">
                      {reconnectTarget.status === "PendingSetup" ? "Setup" : "Reconnect"} {reconnectTarget.name}
                    </h3>
                    <p className="text-[11px] text-muted-foreground">Provide your API access token</p>
                  </div>
                </div>
                <button onClick={() => setReconnectTarget(null)} className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted transition-colors">
                  <X className="w-4 h-4" />
                </button>
              </div>
              <div className="p-5 space-y-4">
                {reconnectError && (
                  <div className="p-3 rounded-lg bg-destructive/10 border border-destructive/20 text-destructive text-xs">{reconnectError}</div>
                )}
                <div>
                  <label className="text-xs font-medium text-foreground mb-1.5 block">Access Token</label>
                  <input
                    type="password"
                    placeholder="ghp_xxxxx, sk-xxxxx, etc."
                    value={reconnectToken}
                    onChange={(e) => setReconnectToken(e.target.value)}
                    className={inputClass}
                    autoFocus
                  />
                  <p className="text-[10px] text-muted-foreground mt-1.5">Your token is encrypted and stored securely.</p>
                </div>
                <div className="flex gap-2 pt-2">
                  <button onClick={() => setReconnectTarget(null)} className="flex-1 h-10 rounded-lg border border-border text-sm text-foreground hover:bg-muted transition-colors">Cancel</button>
                  <motion.button
                    whileTap={{ scale: 0.98 }}
                    onClick={handleReconnect}
                    disabled={reconnectLoading || !reconnectToken.trim()}
                    className="flex-1 h-10 rounded-lg bg-primary text-primary-foreground text-sm font-medium shadow-md shadow-primary/20 disabled:opacity-50 flex items-center justify-center gap-2"
                  >
                    {reconnectLoading ? <><Loader2 className="w-4 h-4 animate-spin" />Connecting...</> : reconnectTarget.status === "PendingSetup" ? "Activate" : "Reconnect"}
                  </motion.button>
                </div>
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>

      {/* ─── Toggle to Workspace Dialog ─── */}
      <AnimatePresence>
        {toggleTarget && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 z-50 flex items-center justify-center">
            <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={() => setToggleTarget(null)} />
            <motion.div
              initial={{ opacity: 0, scale: 0.95, y: 10 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.95, y: 10 }}
              className="relative w-full max-w-sm bg-card border border-border rounded-2xl shadow-2xl overflow-hidden"
            >
              <div className="flex items-center justify-between p-5 border-b border-border">
                <div>
                  <h3 className="text-sm font-semibold text-foreground">Link to Workspace</h3>
                  <p className="text-[11px] text-muted-foreground">Choose where to use {toggleTarget.name}</p>
                </div>
                <button onClick={() => setToggleTarget(null)} className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted transition-colors">
                  <X className="w-4 h-4" />
                </button>
              </div>
              <div className="p-5 space-y-3">
                {workspaces.map((ws) => {
                  const alreadyBound = ws.activeIntegrations?.some((ai) => ai.integrationId === toggleTarget.id);
                  return (
                    <button
                      key={ws.id}
                      onClick={() => setToggleWorkspaceId(ws.id)}
                      className={`w-full flex items-center gap-3 p-3 rounded-lg border transition-all text-left ${
                        toggleWorkspaceId === ws.id
                          ? "border-primary bg-primary/5"
                          : alreadyBound
                          ? "border-emerald-500/30 bg-emerald-500/5"
                          : "border-border hover:border-primary/20"
                      }`}
                    >
                      <Star className={`w-4 h-4 shrink-0 ${ws.isDefault ? "text-amber-500 fill-amber-500" : "text-muted-foreground/30"}`} />
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium text-foreground truncate">{ws.name}</p>
                        <p className="text-[10px] text-muted-foreground">{ws.activeIntegrations?.length || 0} integrations</p>
                      </div>
                      {alreadyBound && (
                        <span className="text-[9px] font-medium text-emerald-600 bg-emerald-500/10 px-1.5 py-0.5 rounded-full">Linked</span>
                      )}
                    </button>
                  );
                })}
                <div className="flex gap-2 pt-2">
                  <button onClick={() => setToggleTarget(null)} className="flex-1 h-10 rounded-lg border border-border text-sm text-foreground hover:bg-muted transition-colors">Cancel</button>
                  <motion.button
                    whileTap={{ scale: 0.98 }}
                    onClick={handleToggleToWorkspace}
                    disabled={toggleLoading || !toggleWorkspaceId}
                    className="flex-1 h-10 rounded-lg bg-primary text-primary-foreground text-sm font-medium shadow-md shadow-primary/20 disabled:opacity-50 flex items-center justify-center gap-2"
                  >
                    {toggleLoading ? <Loader2 className="w-4 h-4 animate-spin" /> : "Link"}
                  </motion.button>
                </div>
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>

      {/* ─── Connect New Integration Dialog ─── */}
      <AnimatePresence>
        {connectTarget && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 z-50 flex items-center justify-center">
            <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={() => setConnectTarget(null)} />
            <motion.div
              initial={{ opacity: 0, scale: 0.95, y: 10 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.95, y: 10 }}
              className="relative w-full max-w-md bg-card border border-border rounded-2xl shadow-2xl overflow-hidden"
            >
              <div className="flex items-center justify-between p-5 border-b border-border">
                <div className="flex items-center gap-3">
                  <div className="w-9 h-9 rounded-lg bg-primary/10 flex items-center justify-center">
                    {(() => { const Svg = getProviderIcon(connectTarget.provider); return Svg ? <Svg size={20} /> : <Plug className="w-5 h-5 text-primary" />; })()}
                  </div>
                  <div>
                    <h3 className="text-sm font-semibold text-foreground">Connect {connectTarget.name}</h3>
                    <p className="text-[11px] text-muted-foreground">{connectTarget.description}</p>
                  </div>
                </div>
                <button onClick={() => setConnectTarget(null)} className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted transition-colors">
                  <X className="w-4 h-4" />
                </button>
              </div>
              <div className="p-5 space-y-4">
                {connectError && (
                  <div className="p-3 rounded-lg bg-destructive/10 border border-destructive/20 text-destructive text-xs">{connectError}</div>
                )}
                <div>
                  <label className="text-xs font-medium text-foreground mb-1.5 block">Access Token</label>
                  <input
                    type="password"
                    placeholder={`Enter your ${connectTarget.name} API token`}
                    value={connectToken}
                    onChange={(e) => setConnectToken(e.target.value)}
                    className={inputClass}
                    autoFocus
                  />
                  <p className="text-[10px] text-muted-foreground mt-1.5">
                    Your token is encrypted and stored securely on our servers.
                    {connectTarget.apiUrl && (
                      <a href={connectTarget.apiUrl} target="_blank" rel="noopener noreferrer" className="text-primary ml-1 hover:underline">
                        Get token →
                      </a>
                    )}
                  </p>
                </div>
                <div className="flex gap-2 pt-2">
                  <button onClick={() => setConnectTarget(null)} className="flex-1 h-10 rounded-lg border border-border text-sm text-foreground hover:bg-muted transition-colors">Cancel</button>
                  <motion.button
                    whileTap={{ scale: 0.98 }}
                    onClick={handleConnect}
                    disabled={connectLoading || !connectToken.trim()}
                    className="flex-1 h-10 rounded-lg bg-primary text-primary-foreground text-sm font-medium shadow-md shadow-primary/20 disabled:opacity-50 flex items-center justify-center gap-2"
                  >
                    {connectLoading ? <><Loader2 className="w-4 h-4 animate-spin" />Connecting...</> : <><Plug className="w-4 h-4" />Connect</>}
                  </motion.button>
                </div>
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
};

export default IntegrationsPanel;
