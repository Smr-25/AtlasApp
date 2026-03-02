import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import {
  Layers, Loader2, Eye, X, ChevronRight,
  SquareKanban, GitBranch, MessageSquare, FileText,
  Calendar, Bug, Phone,
} from "lucide-react";
import { leaderModalsApi, LeaderModalDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const modalTypeIcons: Record<string, typeof SquareKanban> = {
  JiraBoard: SquareKanban,
  GitHubPulse: GitBranch,
  SlackChannels: MessageSquare,
  NotionDocs: FileText,
  CalendarSync: Calendar,
  SentryFeed: Bug,
  PagerDutyOnCall: Phone,
};

const modalTypeColors: Record<string, string> = {
  JiraBoard: "text-blue-400 bg-blue-500/10 border-blue-500/20",
  GitHubPulse: "text-slate-300 bg-slate-500/10 border-slate-500/20",
  SlackChannels: "text-purple-400 bg-purple-500/10 border-purple-500/20",
  NotionDocs: "text-foreground bg-muted/30 border-border/30",
  CalendarSync: "text-emerald-400 bg-emerald-500/10 border-emerald-500/20",
  SentryFeed: "text-red-400 bg-red-500/10 border-red-500/20",
  PagerDutyOnCall: "text-amber-400 bg-amber-500/10 border-amber-500/20",
};

const LeaderModalsPanel = () => {
  const [modals, setModals] = useState<LeaderModalDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedModal, setSelectedModal] = useState<LeaderModalDto | null>(null);
  const [payload, setPayload] = useState<string | null>(null);
  const [payloadLoading, setPayloadLoading] = useState(false);
  const [dismissLoading, setDismissLoading] = useState<string | null>(null);

  const fetchModals = async () => {
    setLoading(true);
    try {
      const r = await leaderModalsApi.getAll();
      if (r.data.isSuccess && r.data.data) setModals(r.data.data);
    } catch {}
    setLoading(false);
  };

  useEffect(() => { fetchModals(); }, []);

  const handleViewPayload = async (modal: LeaderModalDto) => {
    setSelectedModal(modal);
    if (modal.payloadJson) {
      setPayload(modal.payloadJson);
      return;
    }
    setPayloadLoading(true);
    try {
      const r = await leaderModalsApi.getPayload(modal.id);
      if (r.data.isSuccess && r.data.data) {
        setPayload(r.data.data.payloadJson);
      }
    } catch {}
    setPayloadLoading(false);
  };

  const handleDismiss = async (modalId: string) => {
    setDismissLoading(modalId);
    try {
      await leaderModalsApi.dismiss(modalId);
      setModals((prev) => prev.filter((m) => m.id !== modalId));
      if (selectedModal?.id === modalId) {
        setSelectedModal(null);
        setPayload(null);
      }
    } catch {}
    setDismissLoading(null);
  };

  const formatPayload = (json: string | null) => {
    if (!json) return null;
    try { return JSON.parse(json); } catch { return json; }
  };

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-primary/20 to-primary/5 border border-primary/10 flex items-center justify-center">
            <Layers className="w-5 h-5 text-primary" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-foreground tracking-tight">Leader Modals</h2>
            <p className="text-xs text-muted-foreground">Integration data panels & notifications</p>
          </div>
        </div>
        <span className="text-xs text-muted-foreground bg-muted/30 px-2 py-1 rounded-lg">
          {modals.filter((m) => !m.dismissedAt).length} active
        </span>
      </motion.div>

      {loading ? (
        <div className="flex items-center justify-center py-12">
          <Loader2 className="w-5 h-5 animate-spin text-primary" />
        </div>
      ) : modals.length === 0 ? (
        <motion.div variants={item} className="text-center py-16">
          <Layers className="w-8 h-8 text-muted-foreground/20 mx-auto mb-3" />
          <p className="text-sm text-muted-foreground">No active modals</p>
          <p className="text-xs text-muted-foreground/50 mt-1">Integration notifications will appear here</p>
        </motion.div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-3">
          {modals.map((modal) => {
            const Icon = modalTypeIcons[modal.modalType] || Layers;
            const colorClass = modalTypeColors[modal.modalType] || "text-muted-foreground bg-muted/20 border-border/30";
            const isDismissed = !!modal.dismissedAt;

            return (
              <motion.div
                key={modal.id}
                variants={item}
                className={`relative overflow-hidden rounded-xl border p-4 transition-all ${
                  isDismissed ? "opacity-50 border-border/20 bg-card/20" : `border-border/40 bg-card/40 hover:border-primary/15`
                }`}
              >
                <div className="flex items-start justify-between gap-3">
                  <div className="flex items-center gap-3 min-w-0">
                    <div className={`w-9 h-9 rounded-lg flex items-center justify-center shrink-0 border ${colorClass}`}>
                      <Icon className="w-4 h-4" />
                    </div>
                    <div className="min-w-0">
                      <p className="text-sm font-semibold text-foreground">{modal.modalType.replace(/([A-Z])/g, " $1").trim()}</p>
                      <p className="text-[10px] text-muted-foreground mt-0.5">
                        {!modal.hasBeenSeen && <span className="inline-flex items-center gap-1 text-primary mr-2">● New</span>}
                        {isDismissed ? "Dismissed" : "Active"}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-1 shrink-0">
                    <button
                      onClick={() => handleViewPayload(modal)}
                      className="w-7 h-7 rounded-lg flex items-center justify-center text-muted-foreground hover:text-primary hover:bg-primary/5 transition-colors"
                    >
                      <Eye className="w-3.5 h-3.5" />
                    </button>
                    {!isDismissed && (
                      <button
                        onClick={() => handleDismiss(modal.id)}
                        disabled={dismissLoading === modal.id}
                        className="w-7 h-7 rounded-lg flex items-center justify-center text-muted-foreground hover:text-red-400 hover:bg-red-500/5 transition-colors disabled:opacity-50"
                      >
                        {dismissLoading === modal.id ? <Loader2 className="w-3 h-3 animate-spin" /> : <X className="w-3.5 h-3.5" />}
                      </button>
                    )}
                  </div>
                </div>
              </motion.div>
            );
          })}
        </div>
      )}

      {/* Payload Detail Panel */}
      <AnimatePresence>
        {selectedModal && (
          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 10 }}
            className="rounded-xl border border-border/40 bg-card/50 p-5 space-y-3"
          >
            <div className="flex items-center justify-between">
              <h3 className="text-sm font-semibold text-foreground flex items-center gap-2">
                <ChevronRight className="w-4 h-4 text-primary" />
                {selectedModal.modalType.replace(/([A-Z])/g, " $1").trim()} — Payload
              </h3>
              <button onClick={() => { setSelectedModal(null); setPayload(null); }} className="text-muted-foreground hover:text-foreground">
                <X className="w-4 h-4" />
              </button>
            </div>
            {payloadLoading ? (
              <div className="flex items-center justify-center py-8">
                <Loader2 className="w-4 h-4 animate-spin text-primary" />
              </div>
            ) : payload ? (
              <pre className="text-xs text-foreground/80 bg-muted/20 border border-border/20 rounded-lg p-3 overflow-x-auto max-h-64 scrollbar-thin">
                {typeof formatPayload(payload) === "object"
                  ? JSON.stringify(formatPayload(payload), null, 2)
                  : payload}
              </pre>
            ) : (
              <p className="text-xs text-muted-foreground py-4 text-center">No payload data available</p>
            )}
          </motion.div>
        )}
      </AnimatePresence>
    </motion.div>
  );
};

export default LeaderModalsPanel;

