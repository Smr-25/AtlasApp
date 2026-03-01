import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Container, Play, Square, RefreshCw, FileText, Loader2, X, CheckCircle, AlertCircle, Circle } from "lucide-react";
import { dockerApi, DockerContainerDto } from "@/services/api";

const statusConfig: Record<string, { icon: typeof CheckCircle; color: string; bg: string }> = {
  running: { icon: CheckCircle, color: "text-emerald-500", bg: "bg-emerald-500" },
  exited: { icon: Circle, color: "text-zinc-400", bg: "bg-zinc-400" },
  paused: { icon: AlertCircle, color: "text-amber-500", bg: "bg-amber-500" },
  restarting: { icon: RefreshCw, color: "text-blue-400", bg: "bg-blue-400" },
};

const DockerPanel = () => {
  const [containers, setContainers] = useState<DockerContainerDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState<string | null>(null);
  const [logsTarget, setLogsTarget] = useState<DockerContainerDto | null>(null);
  const [logs, setLogs] = useState("");
  const [logsLoading, setLogsLoading] = useState(false);

  const fetchContainers = async () => {
    setLoading(true);
    try {
      const res = await dockerApi.getAll();
      if (res.data.isSuccess && Array.isArray(res.data.data)) setContainers(res.data.data);
    } catch {}
    setLoading(false);
  };

  useEffect(() => { fetchContainers(); }, []);

  const handleAction = async (id: string, action: "start" | "stop" | "restart") => {
    setActionLoading(id);
    try {
      if (action === "start") await dockerApi.start(id);
      else if (action === "stop") await dockerApi.stop(id);
      else await dockerApi.restart(id);
      await fetchContainers();
    } catch {}
    setActionLoading(null);
  };

  const viewLogs = async (c: DockerContainerDto) => {
    setLogsTarget(c);
    setLogsLoading(true);
    try {
      const res = await dockerApi.getLogs(c.id);
      if (res.data.isSuccess) setLogs(typeof res.data.data === "string" ? res.data.data : JSON.stringify(res.data.data));
      else setLogs("Failed to fetch logs");
    } catch { setLogs("Error fetching logs"); }
    setLogsLoading(false);
  };

  const running = containers.filter((c) => c.state === "running");
  const stopped = containers.filter((c) => c.state !== "running");

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-sky-500/20 to-sky-500/5 border border-sky-500/10 flex items-center justify-center">
            <Container className="w-5 h-5 text-sky-400" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-foreground tracking-tight">Docker Containers</h2>
            <p className="text-xs text-muted-foreground">{running.length} running · {stopped.length} stopped</p>
          </div>
        </div>
        <motion.button whileTap={{ scale: 0.98 }} onClick={fetchContainers} disabled={loading} className="flex items-center gap-2 px-3 h-8 rounded-lg border border-border text-xs text-foreground hover:bg-muted/50 transition-colors">
          <RefreshCw className={`w-3.5 h-3.5 ${loading ? "animate-spin" : ""}`} /> Refresh
        </motion.button>
      </div>

      {loading ? (
        <div className="py-16 flex justify-center"><Loader2 className="w-6 h-6 animate-spin text-primary" /></div>
      ) : containers.length === 0 ? (
        <div className="py-16 text-center">
          <Container className="w-12 h-12 text-muted-foreground/20 mx-auto mb-3" />
          <p className="text-sm font-medium text-foreground mb-1">No containers found</p>
          <p className="text-xs text-muted-foreground">Docker might not be running or no containers exist</p>
        </div>
      ) : (
        <div className="space-y-2">
          {containers.map((c, i) => {
            const cfg = statusConfig[c.state] || statusConfig.exited;
            return (
              <motion.div
                key={c.id}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: i * 0.03 }}
                className="relative overflow-hidden flex items-center gap-4 p-4 bg-gradient-to-br from-card to-card/60 rounded-xl border border-border hover:border-primary/15 transition-all group"
              >
                <div className="absolute top-0 right-0 w-16 h-16 bg-primary/[0.015] rounded-full -translate-y-1/2 translate-x-1/3" />
                <div className={`w-2.5 h-2.5 rounded-full shrink-0 ${cfg.bg}`} />
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2">
                    <p className="text-sm font-medium text-foreground truncate font-mono">{c.name}</p>
                    <span className={`text-[10px] font-medium ${cfg.color}`}>{c.state}</span>
                  </div>
                  <p className="text-[11px] text-muted-foreground truncate">{c.image}{c.ports ? ` · ${c.ports}` : ""}</p>
                </div>
                <div className="flex items-center gap-1 shrink-0">
                  <button onClick={() => viewLogs(c)} className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted transition-colors" title="View Logs">
                    <FileText className="w-4 h-4" />
                  </button>
                  {c.state !== "running" ? (
                    <button onClick={() => handleAction(c.id, "start")} disabled={actionLoading === c.id} className="w-8 h-8 rounded-lg flex items-center justify-center text-emerald-500 hover:bg-emerald-500/10 transition-colors" title="Start">
                      {actionLoading === c.id ? <Loader2 className="w-4 h-4 animate-spin" /> : <Play className="w-4 h-4" />}
                    </button>
                  ) : (
                    <>
                      <button onClick={() => handleAction(c.id, "stop")} disabled={actionLoading === c.id} className="w-8 h-8 rounded-lg flex items-center justify-center text-red-500 hover:bg-red-500/10 transition-colors" title="Stop">
                        {actionLoading === c.id ? <Loader2 className="w-4 h-4 animate-spin" /> : <Square className="w-4 h-4" />}
                      </button>
                      <button onClick={() => handleAction(c.id, "restart")} disabled={actionLoading === c.id} className="w-8 h-8 rounded-lg flex items-center justify-center text-blue-400 hover:bg-blue-500/10 transition-colors opacity-0 group-hover:opacity-100" title="Restart">
                        <RefreshCw className="w-4 h-4" />
                      </button>
                    </>
                  )}
                </div>
              </motion.div>
            );
          })}
        </div>
      )}

      {/* Logs Dialog */}
      <AnimatePresence>
        {logsTarget && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 z-50 flex items-center justify-center">
            <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={() => setLogsTarget(null)} />
            <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.95 }} className="relative w-full max-w-2xl bg-card border border-border rounded-2xl shadow-2xl overflow-hidden">
              <div className="flex items-center justify-between p-4 border-b border-border">
                <div>
                  <h3 className="text-sm font-semibold text-foreground font-mono">{logsTarget.name} — Logs</h3>
                  <p className="text-[11px] text-muted-foreground">{logsTarget.image}</p>
                </div>
                <button onClick={() => setLogsTarget(null)} className="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:bg-muted transition-colors"><X className="w-4 h-4" /></button>
              </div>
              <div className="p-4">
                {logsLoading ? (
                  <div className="py-8 flex justify-center"><Loader2 className="w-5 h-5 animate-spin text-primary" /></div>
                ) : (
                  <pre className="p-3 rounded-lg bg-muted/50 border border-border text-[11px] text-foreground font-mono overflow-auto max-h-96 whitespace-pre-wrap">{logs || "No logs available"}</pre>
                )}
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
};

export default DockerPanel;

