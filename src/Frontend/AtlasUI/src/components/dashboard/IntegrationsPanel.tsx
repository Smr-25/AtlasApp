import { useState } from "react";
import { motion } from "framer-motion";
import {
  Plug,
  AlertCircle,
  RefreshCw,
  Trash2,
  Loader2,
  Check,
  Clock,
  XCircle,
} from "lucide-react";
import { IntegrationDto, integrationApi } from "@/services/api";

interface IntegrationsPanelProps {
  integrations: IntegrationDto[];
  pendingIntegrations: IntegrationDto[];
  onRefresh: () => Promise<void>;
}

const statusConfig: Record<string, { icon: typeof Check; color: string; bg: string; label: string }> = {
  Active: { icon: Check, color: "text-emerald-600", bg: "bg-emerald-500/10", label: "Active" },
  PendingSetup: { icon: Clock, color: "text-amber-600", bg: "bg-amber-500/10", label: "Needs Setup" },
  Expired: { icon: AlertCircle, color: "text-red-500", bg: "bg-red-500/10", label: "Expired" },
  Error: { icon: XCircle, color: "text-red-500", bg: "bg-red-500/10", label: "Error" },
  Disconnected: { icon: Plug, color: "text-zinc-500", bg: "bg-zinc-500/10", label: "Disconnected" },
};

const IntegrationsPanel = ({ integrations, pendingIntegrations, onRefresh }: IntegrationsPanelProps) => {
  const [actionLoading, setActionLoading] = useState<string | null>(null);
  const [filter, setFilter] = useState<"all" | "active" | "pending">("all");

  const allIntegrations = [...integrations, ...pendingIntegrations];
  const filtered =
    filter === "active" ? integrations.filter((i) => i.status === "Active")
    : filter === "pending" ? pendingIntegrations
    : allIntegrations;

  const handleDelete = async (id: string) => {
    setActionLoading(id);
    try {
      await integrationApi.remove(id);
      await onRefresh();
    } finally {
      setActionLoading(null);
    }
  };

  const handleReconnect = async (id: string) => {
    // In a real app, this would open a modal to input new token
    // For now, we just mark it as a placeholder action
    setActionLoading(id);
    try {
      // Placeholder — needs token input flow
      await new Promise((r) => setTimeout(r, 500));
    } finally {
      setActionLoading(null);
    }
  };

  return (
    <div className="space-y-5">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-bold text-foreground">Integrations</h2>
          <p className="text-sm text-muted-foreground">
            {allIntegrations.length} total · {integrations.filter((i) => i.status === "Active").length} active
            {pendingIntegrations.length > 0 && ` · ${pendingIntegrations.length} pending`}
          </p>
        </div>
      </div>

      {/* Pending Alert */}
      {pendingIntegrations.length > 0 && (
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
            <p className="text-[11px] text-muted-foreground">Connect your API tokens to activate these integrations</p>
          </div>
        </motion.div>
      )}

      {/* Filters */}
      <div className="flex gap-1.5 p-1 bg-muted/40 rounded-lg w-fit">
        {(["all", "active", "pending"] as const).map((f) => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={`px-3 py-1.5 rounded-md text-xs font-medium transition-all ${
              filter === f
                ? "bg-card text-foreground shadow-sm"
                : "text-muted-foreground hover:text-foreground"
            }`}
          >
            {f === "all" ? "All" : f === "active" ? "Active" : "Pending"}
            <span className="ml-1 text-[10px] text-muted-foreground">
              ({f === "all" ? allIntegrations.length : f === "active" ? integrations.filter((i) => i.status === "Active").length : pendingIntegrations.length})
            </span>
          </button>
        ))}
      </div>

      {/* Integration List */}
      <div className="space-y-2">
        {filtered.length === 0 ? (
          <div className="py-12 text-center">
            <Plug className="w-10 h-10 text-muted-foreground/20 mx-auto mb-3" />
            <p className="text-sm text-muted-foreground">No integrations found</p>
          </div>
        ) : (
          filtered.map((int, i) => {
            const cfg = statusConfig[int.status] || statusConfig.Disconnected;
            const StatusIcon = cfg.icon;
            return (
              <motion.div
                key={int.id}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: i * 0.03 }}
                className="flex items-center gap-4 p-4 bg-card rounded-xl border border-border hover:border-primary/15 transition-all group"
              >
                {/* Icon */}
                <div className="w-10 h-10 rounded-lg bg-muted/60 flex items-center justify-center shrink-0">
                  <Plug className="w-5 h-5 text-muted-foreground" />
                </div>

                {/* Info */}
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2">
                    <p className="text-sm font-medium text-foreground truncate">{int.name}</p>
                    <span className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-[10px] font-medium ${cfg.bg} ${cfg.color}`}>
                      <StatusIcon className="w-2.5 h-2.5" />
                      {cfg.label}
                    </span>
                  </div>
                  <p className="text-[11px] text-muted-foreground">{int.provider}</p>
                </div>

                {/* Actions */}
                <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                  {(int.status === "Expired" || int.status === "Error") && (
                    <button
                      onClick={() => handleReconnect(int.id)}
                      disabled={actionLoading === int.id}
                      className="w-8 h-8 rounded-lg flex items-center justify-center text-primary hover:bg-primary/10 transition-colors"
                      title="Reconnect"
                    >
                      {actionLoading === int.id ? <Loader2 className="w-4 h-4 animate-spin" /> : <RefreshCw className="w-4 h-4" />}
                    </button>
                  )}
                  <button
                    onClick={() => handleDelete(int.id)}
                    disabled={actionLoading === int.id}
                    className="w-8 h-8 rounded-lg flex items-center justify-center text-red-500 hover:bg-red-500/10 transition-colors"
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
    </div>
  );
};

export default IntegrationsPanel;


