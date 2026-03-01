import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import {
  BarChart3, ShieldAlert, ShieldCheck, Clock, Bug,
  Activity, Shield, Database, Wifi,
} from "lucide-react";
import {
  secOpsInsightsApi, ThreatsBlockedDto, VulnsPatchedDto,
  AvgResponseTimeDto, SecurityScoreDto, ZeroIncidentDto,
  ScannedBytesDto, OpenPortsGraphDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.05 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const SecOpsInsightsPanel = () => {
  const [threats, setThreats] = useState<ThreatsBlockedDto | null>(null);
  const [vulns, setVulns] = useState<VulnsPatchedDto | null>(null);
  const [response, setResponse] = useState<AvgResponseTimeDto | null>(null);
  const [score, setScore] = useState<SecurityScoreDto | null>(null);
  const [streak, setStreak] = useState<ZeroIncidentDto | null>(null);
  const [scanned, setScanned] = useState<ScannedBytesDto | null>(null);
  const [ports, setPorts] = useState<OpenPortsGraphDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      const results = await Promise.allSettled([
        secOpsInsightsApi.threatsBlocked(),
        secOpsInsightsApi.vulnerabilitiesPatched(),
        secOpsInsightsApi.avgResponseTime(),
        secOpsInsightsApi.securityScore(),
        secOpsInsightsApi.zeroIncidentStreak(),
        secOpsInsightsApi.scannedBytes(),
        secOpsInsightsApi.openPortsGraph(),
      ]);
      if (results[0].status === "fulfilled" && results[0].value.data.isSuccess) setThreats(results[0].value.data.data);
      if (results[1].status === "fulfilled" && results[1].value.data.isSuccess) setVulns(results[1].value.data.data);
      if (results[2].status === "fulfilled" && results[2].value.data.isSuccess) setResponse(results[2].value.data.data);
      if (results[3].status === "fulfilled" && results[3].value.data.isSuccess) setScore(results[3].value.data.data);
      if (results[4].status === "fulfilled" && results[4].value.data.isSuccess) setStreak(results[4].value.data.data);
      if (results[5].status === "fulfilled" && results[5].value.data.isSuccess) setScanned(results[5].value.data.data);
      if (results[6].status === "fulfilled" && results[6].value.data.isSuccess) setPorts(results[6].value.data.data);
      setLoading(false);
    };
    load();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-red-500/10 flex items-center justify-center animate-pulse">
            <BarChart3 className="w-5 h-5 text-red-400" />
          </div>
          <p className="text-xs text-muted-foreground font-mono">Loading threat intelligence...</p>
        </div>
      </div>
    );
  }

  const summaryCards = [
    { label: "Threats Blocked", value: threats?.totalBlocked?.toLocaleString() || "0", icon: ShieldAlert, color: "text-red-400", bg: "from-red-500/15 to-red-500/3" },
    { label: "Vulns Patched", value: vulns?.totalPatched?.toString() || "0", icon: Bug, color: "text-amber-400", bg: "from-amber-500/15 to-amber-500/3" },
    { label: "Avg Response", value: response ? `${response.averageMinutes}m` : "—", icon: Clock, color: "text-cyan-400", bg: "from-cyan-500/15 to-cyan-500/3" },
    { label: "Security Score", value: score?.grade || "—", icon: ShieldCheck, color: "text-emerald-400", bg: "from-emerald-500/15 to-emerald-500/3" },
    { label: "0-Incident", value: streak ? `${streak.days}d` : "—", icon: Shield, color: "text-violet-400", bg: "from-violet-500/15 to-violet-500/3" },
    { label: "Data Scanned", value: scanned?.formattedSize || "—", icon: Database, color: "text-blue-400", bg: "from-blue-500/15 to-blue-500/3" },
  ];

  const portEntries = ports?.dataPoints ? Object.entries(ports.dataPoints).sort(([a], [b]) => a.localeCompare(b)) : [];
  const maxPort = portEntries.length > 0 ? Math.max(...portEntries.map(([, v]) => v)) : 1;

  const severityColors: Record<string, string> = { critical: "bg-red-500", high: "bg-orange-500", medium: "bg-amber-500", low: "bg-emerald-500" };

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-red-500/20 to-red-500/5 border border-red-500/10 flex items-center justify-center">
          <BarChart3 className="w-5 h-5 text-red-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Threat Intelligence</h2>
          <p className="text-xs text-muted-foreground font-mono">Real-time security metrics & analytics</p>
        </div>
      </motion.div>

      {/* Summary Cards */}
      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3">
        {summaryCards.map((s, i) => (
          <motion.div key={i} variants={item} className={`p-4 rounded-2xl bg-gradient-to-br ${s.bg} border border-border/15`}>
            <s.icon className={`w-4 h-4 ${s.color} mb-2`} />
            <p className="text-xl font-bold text-foreground font-mono">{s.value}</p>
            <p className="text-[10px] text-muted-foreground mt-0.5">{s.label}</p>
          </motion.div>
        ))}
      </div>

      {/* Threat Breakdown + Vuln Severity */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        {/* Threats */}
        {threats && (
          <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
            <h3 className="text-sm font-bold text-foreground mb-4 flex items-center gap-2">
              <ShieldAlert className="w-4 h-4 text-red-400" /> Threat Breakdown
            </h3>
            <div className="space-y-3">
              {[
                { label: "DDoS", value: threats.ddosBlocked, color: "bg-red-500" },
                { label: "Malware", value: threats.malwareBlocked, color: "bg-orange-500" },
                { label: "Brute Force", value: threats.bruteForceBlocked, color: "bg-amber-500" },
              ].map((t, i) => (
                <div key={i}>
                  <div className="flex items-center justify-between mb-1">
                    <span className="text-xs text-muted-foreground font-mono">{t.label}</span>
                    <span className="text-xs font-bold text-foreground font-mono">{t.value}</span>
                  </div>
                  <div className="h-2 bg-muted/30 rounded-full overflow-hidden">
                    <motion.div initial={{ width: 0 }} animate={{ width: `${(t.value / threats.totalBlocked) * 100}%` }}
                      transition={{ duration: 0.6, delay: 0.2 + i * 0.1 }}
                      className={`h-full rounded-full ${t.color}/70`} />
                  </div>
                </div>
              ))}
            </div>
          </motion.div>
        )}

        {/* Vulns by severity */}
        {vulns && (
          <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
            <h3 className="text-sm font-bold text-foreground mb-4 flex items-center gap-2">
              <Bug className="w-4 h-4 text-amber-400" /> Vulnerabilities Patched
            </h3>
            <div className="space-y-2">
              {(["critical", "high", "medium", "low"] as const).map((sev) => (
                <div key={sev} className="flex items-center gap-3">
                  <div className={`w-2 h-2 rounded-full ${severityColors[sev]}`} />
                  <span className="text-xs text-muted-foreground capitalize w-16 font-mono">{sev}</span>
                  <div className="flex-1 h-2 bg-muted/20 rounded-full overflow-hidden">
                    <motion.div initial={{ width: 0 }} animate={{ width: `${(vulns[sev] / vulns.totalPatched) * 100}%` }}
                      transition={{ duration: 0.5 }}
                      className={`h-full rounded-full ${severityColors[sev]}/60`} />
                  </div>
                  <span className="text-xs font-bold text-foreground font-mono w-8 text-right">{vulns[sev]}</span>
                </div>
              ))}
            </div>
          </motion.div>
        )}
      </div>

      {/* Open Ports Graph */}
      {portEntries.length > 0 && (
        <motion.div variants={item} className="p-5 rounded-2xl bg-card/50 border border-border/30">
          <h3 className="text-sm font-bold text-foreground mb-4 flex items-center gap-2">
            <Wifi className="w-4 h-4 text-cyan-400" /> Open Ports Over Time
          </h3>
          <div className="flex items-end gap-2 h-24">
            {portEntries.map(([date, count], i) => (
              <div key={i} className="flex-1 flex flex-col items-center gap-1">
                <motion.div initial={{ height: 0 }} animate={{ height: `${(count / maxPort) * 100}%` }}
                  transition={{ duration: 0.5, delay: i * 0.1 }}
                  className="w-full bg-gradient-to-t from-cyan-500/60 to-cyan-500/20 rounded-t-lg min-h-[4px]" />
                <span className="text-[8px] text-muted-foreground font-mono">{new Date(date).toLocaleDateString(undefined, { month: "short", day: "numeric" })}</span>
              </div>
            ))}
          </div>
        </motion.div>
      )}

      {/* Recommendations */}
      {score && score.recommendations.length > 0 && (
        <motion.div variants={item} className="p-4 rounded-2xl bg-amber-500/5 border border-amber-500/15">
          <h3 className="text-xs font-bold text-foreground mb-2 flex items-center gap-2">
            <Activity className="w-3.5 h-3.5 text-amber-400" /> Recommendations
          </h3>
          <ul className="space-y-1">
            {score.recommendations.map((r, i) => (
              <li key={i} className="text-xs text-muted-foreground font-mono flex items-start gap-2">
                <span className="text-amber-400 mt-0.5">›</span> {r}
              </li>
            ))}
          </ul>
        </motion.div>
      )}
    </motion.div>
  );
};

export default SecOpsInsightsPanel;

