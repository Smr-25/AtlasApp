import { useState } from "react";
import { motion } from "framer-motion";
import {
  Zap, Wifi, AlertOctagon, Trash2, Mail, Key,
  ShieldBan, RefreshCw, Loader2, Terminal,
} from "lucide-react";
import { secOpsScriptsApi, ScriptOutputDto } from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

type ActiveScript = "quickscan" | "panic" | "wipe" | "phishing" | "ssh" | "firewall" | "dns" | null;

const SecOpsScriptsPanel = () => {
  const [activeScript, setActiveScript] = useState<ActiveScript>(null);
  const [outputs, setOutputs] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState<Record<string, boolean>>({});

  const scripts = [
    { id: "quickscan" as const, label: "Quick Scan", desc: "Network range scan", icon: Wifi, color: "text-cyan-400", gradient: "from-cyan-500/12 to-cyan-500/3", danger: false },
    { id: "panic" as const, label: "Panic Button", desc: "Kill all network connections", icon: AlertOctagon, color: "text-red-400", gradient: "from-red-500/12 to-red-500/3", danger: true },
    { id: "wipe" as const, label: "Local Wipe", desc: "Clear history & credentials", icon: Trash2, color: "text-red-400", gradient: "from-red-500/12 to-red-500/3", danger: true },
    { id: "phishing" as const, label: "Phishing Alert", desc: "Analyze suspicious email", icon: Mail, color: "text-amber-400", gradient: "from-amber-500/12 to-amber-500/3", danger: false },
    { id: "ssh" as const, label: "Rotate SSH Key", desc: "Generate new SSH keypair", icon: Key, color: "text-violet-400", gradient: "from-violet-500/12 to-violet-500/3", danger: false },
    { id: "firewall" as const, label: "Firewall Lockdown", desc: "Block all except specified ports", icon: ShieldBan, color: "text-orange-400", gradient: "from-orange-500/12 to-orange-500/3", danger: true },
    { id: "dns" as const, label: "Flush DNS", desc: "Clear DNS cache", icon: RefreshCw, color: "text-emerald-400", gradient: "from-emerald-500/12 to-emerald-500/3", danger: false },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500/20 to-amber-500/5 border border-amber-500/10 flex items-center justify-center">
          <Zap className="w-5 h-5 text-amber-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Security Scripts</h2>
          <p className="text-xs text-muted-foreground font-mono">Automated security operations & incident response</p>
        </div>
      </motion.div>

      {/* Script Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
        {scripts.map((s) => (
          <motion.button key={s.id} variants={item} whileHover={{ y: -2 }} whileTap={{ scale: 0.98 }}
            onClick={() => setActiveScript(activeScript === s.id ? null : s.id)}
            className={`group p-4 rounded-2xl bg-gradient-to-br ${s.gradient} border text-left transition-all duration-200 ${activeScript === s.id ? "border-primary/30 shadow-lg" : "border-border/20 hover:border-primary/15"}`}>
            <div className="flex items-center gap-3 mb-2">
              <s.icon className={`w-5 h-5 ${s.color}`} />
              <span className="text-sm font-semibold text-foreground">{s.label}</span>
              {s.danger && <span className="text-[8px] bg-red-500/15 text-red-400 px-1.5 py-0.5 rounded font-bold font-mono ml-auto">DANGER</span>}
            </div>
            <p className="text-xs text-muted-foreground font-mono">{s.desc}</p>
          </motion.button>
        ))}
      </div>

      {/* Script Forms */}
      {activeScript === "quickscan" && <QuickScanForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "panic" && <PanicForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "wipe" && <WipeForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "phishing" && <PhishingForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "ssh" && <SshForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "firewall" && <FirewallForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
      {activeScript === "dns" && <DnsForm outputs={outputs} setOutputs={setOutputs} loading={loading} setLoading={setLoading} />}
    </motion.div>
  );
};

// ─── Shared Types ──────────────────────────────────────────────
type FormProps = {
  outputs: Record<string, string>;
  setOutputs: React.Dispatch<React.SetStateAction<Record<string, string>>>;
  loading: Record<string, boolean>;
  setLoading: React.Dispatch<React.SetStateAction<Record<string, boolean>>>;
};

const ScriptPanel = ({ title, children, output }: { title: string; children: React.ReactNode; output?: string }) => (
  <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-3">
    <h3 className="text-sm font-bold text-foreground font-mono flex items-center gap-2"><Terminal className="w-4 h-4 text-primary" />{title}</h3>
    {children}
    {output && (
      <pre className="p-3 rounded-xl bg-black/60 border border-border/20 text-xs font-mono text-emerald-400 whitespace-pre-wrap max-h-40 overflow-y-auto">
        {output}
      </pre>
    )}
  </motion.div>
);

const RunBtn = ({ loading, onClick, label, danger }: { loading: boolean; onClick: () => void; label?: string; danger?: boolean }) => (
  <button onClick={onClick} disabled={loading}
    className={`h-9 px-4 rounded-xl text-xs font-semibold font-mono disabled:opacity-50 transition-colors ${danger ? "bg-red-500 text-white hover:bg-red-600" : "bg-primary text-primary-foreground hover:bg-primary/90"}`}>
    {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : label || "Execute"}
  </button>
);

// ─── Quick Scan ────────────────────────────────────────────────
const QuickScanForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [range, setRange] = useState("192.168.1.0/24");
  const run = async () => {
    setLoading(p => ({ ...p, quickscan: true }));
    try { const r = await secOpsScriptsApi.quickScan({ networkRange: range }); if (r.data.isSuccess) setOutputs(p => ({ ...p, quickscan: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, quickscan: false }));
  };
  return (
    <ScriptPanel title="Quick Network Scan" output={outputs.quickscan}>
      <div className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-[10px] text-muted-foreground font-mono">Network Range (CIDR)</label>
          <input type="text" value={range} onChange={e => setRange(e.target.value)}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono"
            onKeyDown={e => e.key === "Enter" && run()} />
        </div>
        <RunBtn loading={loading.quickscan} onClick={run} label="Scan" />
      </div>
    </ScriptPanel>
  );
};

// ─── Panic Button ──────────────────────────────────────────────
const PanicForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [iface, setIface] = useState("en0");
  const run = async () => {
    if (!confirm("⚠️ This will KILL all network connections. Continue?")) return;
    setLoading(p => ({ ...p, panic: true }));
    try { const r = await secOpsScriptsApi.panicButton({ interfaceName: iface }); if (r.data.isSuccess) setOutputs(p => ({ ...p, panic: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, panic: false }));
  };
  return (
    <ScriptPanel title="🚨 Panic Button" output={outputs.panic}>
      <div className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-[10px] text-muted-foreground font-mono">Network Interface</label>
          <input type="text" value={iface} onChange={e => setIface(e.target.value)}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono" />
        </div>
        <RunBtn loading={loading.panic} onClick={run} label="KILL NETWORK" danger />
      </div>
    </ScriptPanel>
  );
};

// ─── Local Wipe ────────────────────────────────────────────────
const WipeForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [wipeHistory, setWipeHistory] = useState(true);
  const [wipeCreds, setWipeCreds] = useState(false);
  const run = async () => {
    if (wipeCreds && !confirm("⚠️ This will erase stored credentials. Continue?")) return;
    setLoading(p => ({ ...p, wipe: true }));
    try { const r = await secOpsScriptsApi.localWipe({ wipeHistory, wipeCredentials: wipeCreds }); if (r.data.isSuccess) setOutputs(p => ({ ...p, wipe: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, wipe: false }));
  };
  return (
    <ScriptPanel title="Local Data Wipe" output={outputs.wipe}>
      <div className="flex items-center gap-4">
        <label className="flex items-center gap-2 text-xs font-mono text-foreground">
          <input type="checkbox" checked={wipeHistory} onChange={e => setWipeHistory(e.target.checked)} className="rounded" /> History
        </label>
        <label className="flex items-center gap-2 text-xs font-mono text-foreground">
          <input type="checkbox" checked={wipeCreds} onChange={e => setWipeCreds(e.target.checked)} className="rounded" /> Credentials
        </label>
        <div className="flex-1" />
        <RunBtn loading={loading.wipe} onClick={run} label="Wipe" danger />
      </div>
    </ScriptPanel>
  );
};

// ─── Phishing Alert ────────────────────────────────────────────
const PhishingForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [headers, setHeaders] = useState("");
  const [sender, setSender] = useState("");
  const run = async () => {
    if (!headers.trim() || !sender.trim()) return;
    setLoading(p => ({ ...p, phish: true }));
    try { const r = await secOpsScriptsApi.phishingAlert({ emailHeaders: headers, senderAddress: sender }); if (r.data.isSuccess) setOutputs(p => ({ ...p, phish: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, phish: false }));
  };
  return (
    <ScriptPanel title="Phishing Email Analysis" output={outputs.phish}>
      <div className="space-y-2">
        <div className="space-y-1">
          <label className="text-[10px] text-muted-foreground font-mono">Sender Address</label>
          <input type="text" value={sender} onChange={e => setSender(e.target.value)} placeholder="support@g00gle.com"
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono placeholder:text-muted-foreground/40" />
        </div>
        <div className="space-y-1">
          <label className="text-[10px] text-muted-foreground font-mono">Email Headers</label>
          <textarea value={headers} onChange={e => setHeaders(e.target.value)} rows={3} placeholder="Paste email headers..."
            className="w-full px-3 py-2 rounded-xl bg-muted/30 border border-border/30 text-xs font-mono text-foreground placeholder:text-muted-foreground/40 resize-none" />
        </div>
        <RunBtn loading={loading.phish} onClick={run} label="Analyze" />
      </div>
    </ScriptPanel>
  );
};

// ─── SSH Rotate ────────────────────────────────────────────────
const SshForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [comment, setComment] = useState("user@atlas-prod");
  const [size, setSize] = useState("4096");
  const run = async () => {
    setLoading(p => ({ ...p, ssh: true }));
    try { const r = await secOpsScriptsApi.rotateSsh({ keyComment: comment, keySize: Number(size) }); if (r.data.isSuccess) setOutputs(p => ({ ...p, ssh: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, ssh: false }));
  };
  return (
    <ScriptPanel title="Rotate SSH Key" output={outputs.ssh}>
      <div className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-[10px] text-muted-foreground font-mono">Key Comment</label>
          <input type="text" value={comment} onChange={e => setComment(e.target.value)}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono" />
        </div>
        <div className="w-24 space-y-1">
          <label className="text-[10px] text-muted-foreground font-mono">Key Size</label>
          <select value={size} onChange={e => setSize(e.target.value)} className="w-full h-9 px-2 rounded-xl bg-muted/30 border border-border/30 text-xs text-foreground font-mono">
            <option value="2048">2048</option><option value="4096">4096</option>
          </select>
        </div>
        <RunBtn loading={loading.ssh} onClick={run} label="Generate" />
      </div>
    </ScriptPanel>
  );
};

// ─── Firewall Lockdown ─────────────────────────────────────────
const FirewallForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const [ports, setPorts] = useState("22, 443, 5432");
  const run = async () => {
    if (!confirm("⚠️ This will block all traffic except specified ports. Continue?")) return;
    const allowedPorts = ports.split(",").map(p => Number(p.trim())).filter(Boolean);
    setLoading(p => ({ ...p, fw: true }));
    try { const r = await secOpsScriptsApi.firewallLockdown({ allowedPorts }); if (r.data.isSuccess) setOutputs(p => ({ ...p, fw: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, fw: false }));
  };
  return (
    <ScriptPanel title="Firewall Lockdown" output={outputs.fw}>
      <div className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-[10px] text-muted-foreground font-mono">Allowed Ports (comma separated)</label>
          <input type="text" value={ports} onChange={e => setPorts(e.target.value)}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono" />
        </div>
        <RunBtn loading={loading.fw} onClick={run} label="LOCKDOWN" danger />
      </div>
    </ScriptPanel>
  );
};

// ─── DNS Flush ─────────────────────────────────────────────────
const DnsForm = ({ outputs, setOutputs, loading, setLoading }: FormProps) => {
  const run = async () => {
    setLoading(p => ({ ...p, dns: true }));
    try { const r = await secOpsScriptsApi.clearDns(); if (r.data.isSuccess) setOutputs(p => ({ ...p, dns: r.data.data.output })); } catch {}
    setLoading(p => ({ ...p, dns: false }));
  };
  return (
    <ScriptPanel title="Flush DNS Cache" output={outputs.dns}>
      <RunBtn loading={loading.dns} onClick={run} label="Flush DNS" />
    </ScriptPanel>
  );
};

export default SecOpsScriptsPanel;

