import { useState } from "react";
import { motion } from "framer-motion";
import {
  Bot, Wifi, ShieldAlert, Lock, Bug, Skull,
  Eye, Loader2, AlertTriangle, CheckCircle2,
} from "lucide-react";
import {
  secOpsAgentsApi, RoguePortDto, ExpiringSslDto,
  SuspiciousTrafficDto, LeakedKeyDto, PatchSuggestionDto,
  ZombieProcessDto, VpnStatusDto,
} from "@/services/api";
import { useEffect } from "react";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

const SecOpsAgentsPanel = () => {
  const [vpn, setVpn] = useState<VpnStatusDto | null>(null);
  const [roguePorts, setRoguePorts] = useState<RoguePortDto[] | null>(null);
  const [expiringSsl, setExpiringSsl] = useState<ExpiringSslDto[] | null>(null);
  const [traffic, setTraffic] = useState<SuspiciousTrafficDto | null>(null);
  const [leakedKeys, setLeakedKeys] = useState<LeakedKeyDto[] | null>(null);
  const [patches, setPatches] = useState<PatchSuggestionDto[] | null>(null);
  const [zombies, setZombies] = useState<ZombieProcessDto[] | null>(null);
  const [loading, setLoading] = useState<Record<string, boolean>>({});

  // Auto-load VPN status
  useEffect(() => {
    secOpsAgentsApi.vpnStatus().then(r => { if (r.data.isSuccess) setVpn(r.data.data); }).catch(() => {});
  }, []);

  const runAgent = async (id: string) => {
    setLoading(prev => ({ ...prev, [id]: true }));
    try {
      switch (id) {
        case "rogue": {
          const r = await secOpsAgentsApi.detectRoguePorts();
          if (r.data.isSuccess) setRoguePorts(r.data.data);
          break;
        }
        case "ssl": {
          const r = await secOpsAgentsApi.warnExpiringSsl({ domains: ["example.com", "api.example.com"] });
          if (r.data.isSuccess) setExpiringSsl(r.data.data);
          break;
        }
        case "traffic": {
          const r = await secOpsAgentsApi.detectSuspiciousTraffic({ targetUrl: "https://api.example.com" });
          if (r.data.isSuccess) setTraffic(r.data.data);
          break;
        }
        case "keys": {
          const r = await secOpsAgentsApi.scanLeakedKeys({ content: "// check current codebase" });
          if (r.data.isSuccess) setLeakedKeys(r.data.data);
          break;
        }
        case "patches": {
          const r = await secOpsAgentsApi.suggestPatches({ projectPath: "." });
          if (r.data.isSuccess) setPatches(r.data.data);
          break;
        }
        case "zombies": {
          const r = await secOpsAgentsApi.killZombieProcesses();
          if (r.data.isSuccess) setZombies(r.data.data);
          break;
        }
      }
    } catch {}
    setLoading(prev => ({ ...prev, [id]: false }));
  };

  const agents = [
    { id: "rogue", label: "Detect Rogue Ports", desc: "Scan for suspicious open ports", icon: Wifi, color: "text-red-400", gradient: "from-red-500/12 to-red-500/3" },
    { id: "ssl", label: "Expiring SSL", desc: "Warn about expiring certificates", icon: ShieldAlert, color: "text-amber-400", gradient: "from-amber-500/12 to-amber-500/3" },
    { id: "traffic", label: "Suspicious Traffic", desc: "Analyze for anomalies", icon: Eye, color: "text-violet-400", gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "keys", label: "Leaked Key Scan", desc: "Find exposed secrets in code", icon: Lock, color: "text-cyan-400", gradient: "from-cyan-500/12 to-cyan-500/3" },
    { id: "patches", label: "Suggest Patches", desc: "Auto-patch recommendations", icon: Bug, color: "text-emerald-400", gradient: "from-emerald-500/12 to-emerald-500/3" },
    { id: "zombies", label: "Kill Zombies", desc: "Terminate zombie processes", icon: Skull, color: "text-pink-400", gradient: "from-pink-500/12 to-pink-500/3" },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-violet-500/20 to-violet-500/5 border border-violet-500/10 flex items-center justify-center">
          <Bot className="w-5 h-5 text-violet-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">AI Security Agents</h2>
          <p className="text-xs text-muted-foreground font-mono">Automated threat detection & response</p>
        </div>
      </motion.div>

      {/* VPN Status Banner */}
      {vpn && (
        <motion.div variants={item} className={`p-3 rounded-xl border flex items-center gap-3 ${vpn.isConnected && !vpn.isLeaking ? "bg-emerald-500/5 border-emerald-500/15" : "bg-red-500/5 border-red-500/15"}`}>
          <Wifi className={`w-4 h-4 ${vpn.isConnected ? "text-emerald-400" : "text-red-400"}`} />
          <div className="flex-1">
            <p className="text-xs font-bold text-foreground font-mono">
              VPN: {vpn.isConnected ? "Connected" : "Disconnected"}
              {vpn.isLeaking && <span className="text-red-400 ml-2">⚠ DNS LEAK DETECTED</span>}
            </p>
            <p className="text-[10px] text-muted-foreground font-mono">Public IP: {vpn.publicIp}{vpn.vpnIp && ` • VPN IP: ${vpn.vpnIp}`}</p>
          </div>
        </motion.div>
      )}

      {/* Agent Buttons */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
        {agents.map((a) => (
          <motion.button key={a.id} variants={item} whileHover={{ y: -2 }} whileTap={{ scale: 0.98 }}
            onClick={() => runAgent(a.id)} disabled={loading[a.id]}
            className={`group p-4 rounded-2xl bg-gradient-to-br ${a.gradient} border border-border/20 hover:border-primary/15 text-left transition-all duration-200`}>
            <div className="flex items-center gap-3 mb-2">
              {loading[a.id] ? <Loader2 className="w-5 h-5 animate-spin text-muted-foreground" /> : <a.icon className={`w-5 h-5 ${a.color}`} />}
              <span className="text-sm font-semibold text-foreground">{a.label}</span>
            </div>
            <p className="text-xs text-muted-foreground font-mono">{a.desc}</p>
          </motion.button>
        ))}
      </div>

      {/* Results */}
      {roguePorts && (
        <ResultPanel title="Rogue Ports Detected" icon={<Wifi className="w-4 h-4 text-red-400" />}>
          {roguePorts.length === 0 ? <p className="text-xs text-emerald-400 font-mono">✓ No rogue ports found</p> : (
            <div className="space-y-1">
              {roguePorts.map((p, i) => (
                <div key={i} className="flex items-center gap-3 p-2 rounded-lg bg-red-500/5 border border-red-500/10 text-xs font-mono">
                  <span className="text-red-400 font-bold">:{p.port}</span>
                  <span className="text-foreground">{p.processName}</span>
                  <span className="text-muted-foreground">PID:{p.processId}</span>
                  <span className="text-amber-400 ml-auto">{p.status}</span>
                </div>
              ))}
            </div>
          )}
        </ResultPanel>
      )}

      {expiringSsl && (
        <ResultPanel title="Expiring SSL Certificates" icon={<ShieldAlert className="w-4 h-4 text-amber-400" />}>
          {expiringSsl.length === 0 ? <p className="text-xs text-emerald-400 font-mono">✓ All certificates are healthy</p> : (
            <div className="space-y-1">
              {expiringSsl.map((s, i) => (
                <div key={i} className="flex items-center gap-3 p-2 rounded-lg bg-amber-500/5 border border-amber-500/10 text-xs font-mono">
                  <span className="text-foreground font-bold">{s.domain}</span>
                  <span className={`ml-auto font-bold ${s.daysRemaining < 7 ? "text-red-400" : "text-amber-400"}`}>{s.daysRemaining}d left</span>
                </div>
              ))}
            </div>
          )}
        </ResultPanel>
      )}

      {traffic && (
        <ResultPanel title="Traffic Analysis" icon={<Eye className="w-4 h-4 text-violet-400" />}>
          <div className={`p-3 rounded-lg border text-xs font-mono ${traffic.isSuspicious ? "bg-red-500/5 border-red-500/10" : "bg-emerald-500/5 border-emerald-500/10"}`}>
            <div className="flex items-center gap-2 mb-1">
              {traffic.isSuspicious ? <AlertTriangle className="w-3.5 h-3.5 text-red-400" /> : <CheckCircle2 className="w-3.5 h-3.5 text-emerald-400" />}
              <span className={`font-bold ${traffic.isSuspicious ? "text-red-400" : "text-emerald-400"}`}>{traffic.isSuspicious ? "SUSPICIOUS" : "CLEAN"}</span>
            </div>
            <p className="text-muted-foreground">{traffic.summary}</p>
            <p className="text-muted-foreground/50 mt-1">{traffic.requestCount.toLocaleString()} requests • Origin: {traffic.originCountry}</p>
          </div>
        </ResultPanel>
      )}

      {leakedKeys && (
        <ResultPanel title="Leaked Key Scan" icon={<Lock className="w-4 h-4 text-cyan-400" />}>
          {leakedKeys.length === 0 ? <p className="text-xs text-emerald-400 font-mono">✓ No leaked keys found</p> : (
            <div className="space-y-1">
              {leakedKeys.map((k, i) => (
                <div key={i} className="p-2 rounded-lg bg-red-500/5 border border-red-500/10 text-xs font-mono">
                  <span className="text-red-400 font-bold">{k.keyType}</span>
                  <span className="text-muted-foreground ml-2">Line {k.lineNumber}: </span>
                  <code className="text-foreground">{k.snippet}</code>
                </div>
              ))}
            </div>
          )}
        </ResultPanel>
      )}

      {patches && (
        <ResultPanel title="Patch Suggestions" icon={<Bug className="w-4 h-4 text-emerald-400" />}>
          {patches.length === 0 ? <p className="text-xs text-emerald-400 font-mono">✓ All packages up to date</p> : (
            <div className="space-y-1">
              {patches.map((p, i) => {
                const sevColor = p.severity === "Critical" ? "text-red-400" : p.severity === "High" ? "text-orange-400" : "text-amber-400";
                return (
                  <div key={i} className="flex items-center gap-3 p-2 rounded-lg bg-muted/10 border border-border/10 text-xs font-mono">
                    <span className="text-foreground font-bold">{p.packageName}</span>
                    <span className="text-muted-foreground">{p.currentVersion} → {p.suggestedVersion}</span>
                    <span className={`ml-auto font-bold ${sevColor}`}>{p.severity}</span>
                  </div>
                );
              })}
            </div>
          )}
        </ResultPanel>
      )}

      {zombies && (
        <ResultPanel title="Zombie Processes" icon={<Skull className="w-4 h-4 text-pink-400" />}>
          {zombies.length === 0 ? <p className="text-xs text-emerald-400 font-mono">✓ No zombie processes</p> : (
            <div className="space-y-1">
              {zombies.map((z, i) => (
                <div key={i} className="flex items-center gap-3 p-2 rounded-lg bg-muted/10 border border-border/10 text-xs font-mono">
                  <span className="text-foreground">PID:{z.processId}</span>
                  <span className="text-muted-foreground">{z.processName}</span>
                  <span className="text-muted-foreground">{z.memoryMb}MB</span>
                  <span className="text-emerald-400 ml-auto font-bold">{z.status}</span>
                </div>
              ))}
            </div>
          )}
        </ResultPanel>
      )}
    </motion.div>
  );
};

const ResultPanel = ({ title, icon, children }: { title: string; icon: React.ReactNode; children: React.ReactNode }) => (
  <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-4 rounded-2xl bg-card/50 border border-border/30">
    <h3 className="text-xs font-bold text-foreground mb-3 flex items-center gap-2 font-mono">{icon} {title}</h3>
    {children}
  </motion.div>
);

export default SecOpsAgentsPanel;

