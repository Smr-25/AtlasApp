import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { Bug, Shield, Check, Loader2, AlertTriangle, AlertCircle, Info } from "lucide-react";
import { sentryApi, sonarQubeApi, SentryIssueDto, SonarQubeDto, IntegrationDto } from "@/services/api";

interface MonitoringPanelProps {
  integrations: IntegrationDto[];
}

const levelConfig: Record<string, { icon: typeof AlertCircle; color: string; bg: string }> = {
  error: { icon: AlertCircle, color: "text-red-500", bg: "bg-red-500/10" },
  warning: { icon: AlertTriangle, color: "text-amber-500", bg: "bg-amber-500/10" },
  info: { icon: Info, color: "text-blue-400", bg: "bg-blue-500/10" },
  fatal: { icon: AlertCircle, color: "text-red-600", bg: "bg-red-600/10" },
};

const MonitoringPanel = ({ integrations }: MonitoringPanelProps) => {
  const [sentryIssues, setSentryIssues] = useState<SentryIssueDto[]>([]);
  const [sonarData, setSonarData] = useState<SonarQubeDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState<string | null>(null);

  const sentryInt = integrations.find((i) => i.provider === "Sentry" && i.status === "Active");
  const sonarInt = integrations.find((i) => i.provider === "SonarQube" && i.status === "Active");

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      const calls: Promise<any>[] = [];
      if (sentryInt) calls.push(sentryApi.getIssues(sentryInt.id).then((r) => { if (r.data.isSuccess) setSentryIssues(r.data.data || []); }));
      if (sonarInt) calls.push(sonarQubeApi.getQuality(sonarInt.id).then((r) => { if (r.data.isSuccess) setSonarData(r.data.data); }));
      await Promise.allSettled(calls);
      setLoading(false);
    };
    load();
  }, [sentryInt?.id, sonarInt?.id]);

  const handleResolve = async (issueId: string) => {
    setActionLoading(issueId);
    try { await sentryApi.resolve(issueId); setSentryIssues((prev) => prev.filter((i) => i.id !== issueId)); } catch {}
    setActionLoading(null);
  };

  if (loading) return <div className="py-20 flex justify-center"><Loader2 className="w-6 h-6 animate-spin text-primary" /></div>;

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-lg font-bold text-foreground flex items-center gap-2"><Bug className="w-5 h-5 text-primary" /> Monitoring</h2>
        <p className="text-sm text-muted-foreground">Error tracking & code quality</p>
      </div>

      {/* SonarQube Quality Gate */}
      <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="bg-card rounded-xl border border-border p-5">
        <div className="flex items-center gap-2 mb-4">
          <Shield className="w-4 h-4 text-primary" />
          <h3 className="text-sm font-semibold text-foreground">Code Quality {!sonarInt && <span className="text-muted-foreground font-normal">(SonarQube not connected)</span>}</h3>
        </div>
        {sonarData ? (
          <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-3">
            {[
              { label: "Status", value: sonarData.status, color: sonarData.status === "OK" ? "text-emerald-500" : "text-red-500" },
              { label: "Bugs", value: sonarData.bugs.toString(), color: sonarData.bugs === 0 ? "text-emerald-500" : "text-amber-500" },
              { label: "Vulnerabilities", value: sonarData.vulnerabilities.toString(), color: sonarData.vulnerabilities === 0 ? "text-emerald-500" : "text-red-500" },
              { label: "Code Smells", value: sonarData.codeSmells.toString(), color: "text-foreground" },
              { label: "Coverage", value: `${sonarData.coverage}%`, color: sonarData.coverage >= 80 ? "text-emerald-500" : sonarData.coverage >= 50 ? "text-amber-500" : "text-red-500" },
            ].map((m) => (
              <div key={m.label} className="text-center">
                <p className={`text-xl font-bold ${m.color}`}>{m.value}</p>
                <p className="text-[10px] text-muted-foreground mt-0.5">{m.label}</p>
              </div>
            ))}
          </div>
        ) : (
          <p className="text-xs text-muted-foreground text-center py-4">Connect SonarQube integration to see code quality metrics</p>
        )}
      </motion.div>

      {/* Sentry Issues */}
      <div>
        <div className="flex items-center gap-2 mb-3">
          <Bug className="w-4 h-4 text-primary" />
          <h3 className="text-sm font-semibold text-foreground">Sentry Issues {!sentryInt && <span className="text-muted-foreground font-normal">(not connected)</span>}</h3>
          {sentryIssues.length > 0 && <span className="text-[10px] bg-red-500/10 text-red-500 px-1.5 py-0.5 rounded-full font-medium">{sentryIssues.length}</span>}
        </div>

        {!sentryInt ? (
          <div className="py-8 text-center bg-card rounded-xl border border-border">
            <Bug className="w-10 h-10 text-muted-foreground/20 mx-auto mb-2" />
            <p className="text-xs text-muted-foreground">Connect Sentry integration to track errors</p>
          </div>
        ) : sentryIssues.length === 0 ? (
          <div className="py-8 text-center bg-card rounded-xl border border-border">
            <Check className="w-10 h-10 text-emerald-500/30 mx-auto mb-2" />
            <p className="text-sm font-medium text-foreground">All clear! 🎉</p>
            <p className="text-xs text-muted-foreground">No unresolved issues</p>
          </div>
        ) : (
          <div className="space-y-2">
            {sentryIssues.map((issue, i) => {
              const cfg = levelConfig[issue.level] || levelConfig.info;
              const LevelIcon = cfg.icon;
              return (
                <motion.div key={issue.id} initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.03 }} className="flex items-start gap-3 p-3 bg-card rounded-xl border border-border hover:border-primary/15 transition-all group">
                  <div className={`w-8 h-8 rounded-lg flex items-center justify-center shrink-0 mt-0.5 ${cfg.bg}`}>
                    <LevelIcon className={`w-4 h-4 ${cfg.color}`} />
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-foreground">{issue.title}</p>
                    <p className="text-[11px] text-muted-foreground truncate">{issue.culprit}</p>
                    <div className="flex items-center gap-3 mt-1 text-[10px] text-muted-foreground">
                      <span>{issue.count} events</span>
                      <span>Last: {new Date(issue.lastSeen).toLocaleDateString()}</span>
                    </div>
                  </div>
                  <motion.button whileTap={{ scale: 0.95 }} onClick={() => handleResolve(issue.id)} disabled={actionLoading === issue.id} className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium text-emerald-600 bg-emerald-500/10 hover:bg-emerald-500/20 transition-colors shrink-0 opacity-0 group-hover:opacity-100">
                    {actionLoading === issue.id ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Check className="w-3.5 h-3.5" />} Resolve
                  </motion.button>
                </motion.div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
};

export default MonitoringPanel;

