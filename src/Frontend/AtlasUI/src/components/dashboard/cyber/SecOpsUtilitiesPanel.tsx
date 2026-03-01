import { useState } from "react";
import { motion } from "framer-motion";
import {
  Wrench, Hash, Globe, Lock, ShieldCheck, Wifi,
  Copy, Check, Loader2,
} from "lucide-react";
import {
  secOpsUtilitiesApi, HashResultDto, IpDnsResultDto,
  EncodeResultDto, PasswordEntropyDto, SslCheckDto, PortScanResultDto,
} from "@/services/api";

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.04 } } };
const item = { hidden: { opacity: 0, y: 12 }, show: { opacity: 1, y: 0 } };

type ActiveTool = "hash" | "ipdns" | "encode" | "password" | "ssl" | "portscan" | null;

const SecOpsUtilitiesPanel = () => {
  const [activeTool, setActiveTool] = useState<ActiveTool>(null);

  const tools = [
    { id: "hash" as const, label: "Hash Generator", desc: "SHA256, SHA512, MD5, SHA1", icon: Hash, color: "text-cyan-400", gradient: "from-cyan-500/12 to-cyan-500/3" },
    { id: "ipdns" as const, label: "IP / DNS Lookup", desc: "Reverse lookup & geo info", icon: Globe, color: "text-blue-400", gradient: "from-blue-500/12 to-blue-500/3" },
    { id: "encode" as const, label: "Payload Encoder", desc: "Base64, Hex, URL encode", icon: Lock, color: "text-violet-400", gradient: "from-violet-500/12 to-violet-500/3" },
    { id: "password" as const, label: "Password Entropy", desc: "Strength & crack time", icon: Lock, color: "text-amber-400", gradient: "from-amber-500/12 to-amber-500/3" },
    { id: "ssl" as const, label: "SSL Checker", desc: "Certificate validation", icon: ShieldCheck, color: "text-emerald-400", gradient: "from-emerald-500/12 to-emerald-500/3" },
    { id: "portscan" as const, label: "Port Scanner", desc: "Local network port scan", icon: Wifi, color: "text-red-400", gradient: "from-red-500/12 to-red-500/3" },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      <motion.div variants={item} className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-cyan-500/20 to-cyan-500/5 border border-cyan-500/10 flex items-center justify-center">
          <Wrench className="w-5 h-5 text-cyan-400" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-foreground tracking-tight">Security Tools</h2>
          <p className="text-xs text-muted-foreground font-mono">Recon, analysis & validation utilities</p>
        </div>
      </motion.div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
        {tools.map((t) => (
          <motion.button key={t.id} variants={item} whileHover={{ y: -2 }} whileTap={{ scale: 0.98 }}
            onClick={() => setActiveTool(activeTool === t.id ? null : t.id)}
            className={`group p-4 rounded-2xl bg-gradient-to-br ${t.gradient} border text-left transition-all duration-200 ${activeTool === t.id ? "border-primary/30 shadow-lg shadow-primary/5" : "border-border/20 hover:border-primary/15"}`}>
            <div className="flex items-center gap-3 mb-2">
              <t.icon className={`w-5 h-5 ${t.color}`} />
              <span className="text-sm font-semibold text-foreground">{t.label}</span>
            </div>
            <p className="text-xs text-muted-foreground font-mono">{t.desc}</p>
          </motion.button>
        ))}
      </div>

      {activeTool === "hash" && <HashTool />}
      {activeTool === "ipdns" && <IpDnsTool />}
      {activeTool === "encode" && <EncodeTool />}
      {activeTool === "password" && <PasswordTool />}
      {activeTool === "ssl" && <SslTool />}
      {activeTool === "portscan" && <PortScanTool />}
    </motion.div>
  );
};

// ─── Hash Generator ────────────────────────────────────────────
const HashTool = () => {
  const [input, setInput] = useState("");
  const [algo, setAlgo] = useState("SHA256");
  const [result, setResult] = useState<HashResultDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [copied, setCopied] = useState(false);

  const run = async () => {
    if (!input.trim()) return;
    setLoading(true);
    try { const r = await secOpsUtilitiesApi.hash({ input, algorithm: algo }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };

  return (
    <ToolPanel title="Hash Generator">
      <div className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-xs text-muted-foreground font-mono">Input</label>
          <input type="text" value={input} onChange={e => setInput(e.target.value)} placeholder="Text to hash..."
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono placeholder:text-muted-foreground/40"
            onKeyDown={e => e.key === "Enter" && run()} />
        </div>
        <select value={algo} onChange={e => setAlgo(e.target.value)} className="h-9 px-2 rounded-xl bg-muted/30 border border-border/30 text-xs text-foreground font-mono">
          {["SHA256", "SHA512", "MD5", "SHA1"].map(a => <option key={a} value={a}>{a}</option>)}
        </select>
        <RunButton loading={loading} onClick={run} disabled={!input.trim()} />
      </div>
      {result && (
        <div className="relative mt-3">
          <pre className="p-3 rounded-xl bg-muted/20 border border-border/20 text-xs font-mono text-foreground break-all">{result.hash}</pre>
          <CopyBtn text={result.hash} />
        </div>
      )}
    </ToolPanel>
  );
};

// ─── IP / DNS Lookup ───────────────────────────────────────────
const IpDnsTool = () => {
  const [target, setTarget] = useState("");
  const [result, setResult] = useState<IpDnsResultDto | null>(null);
  const [loading, setLoading] = useState(false);

  const run = async () => {
    if (!target.trim()) return;
    setLoading(true);
    try { const r = await secOpsUtilitiesApi.ipDns({ target }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };

  return (
    <ToolPanel title="IP / DNS Lookup">
      <div className="flex items-end gap-3">
        <div className="flex-1 space-y-1">
          <label className="text-xs text-muted-foreground font-mono">IP or Domain</label>
          <input type="text" value={target} onChange={e => setTarget(e.target.value)} placeholder="8.8.8.8 or example.com"
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono placeholder:text-muted-foreground/40"
            onKeyDown={e => e.key === "Enter" && run()} />
        </div>
        <RunButton loading={loading} onClick={run} disabled={!target.trim()} />
      </div>
      {result && (
        <div className="mt-3 grid grid-cols-2 gap-2">
          {Object.entries(result).map(([k, v]) => (
            <div key={k} className="p-2 rounded-lg bg-muted/15 border border-border/10">
              <p className="text-[10px] text-muted-foreground font-mono uppercase">{k}</p>
              <p className="text-xs text-foreground font-mono font-medium mt-0.5">{String(v)}</p>
            </div>
          ))}
        </div>
      )}
    </ToolPanel>
  );
};

// ─── Payload Encoder ───────────────────────────────────────────
const EncodeTool = () => {
  const [input, setInput] = useState("");
  const [encoding, setEncoding] = useState("Base64");
  const [result, setResult] = useState<EncodeResultDto | null>(null);
  const [loading, setLoading] = useState(false);

  const run = async () => {
    if (!input.trim()) return;
    setLoading(true);
    try { const r = await secOpsUtilitiesApi.encodePayload({ input, encoding }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };

  return (
    <ToolPanel title="Payload Encoder">
      <textarea value={input} onChange={e => setInput(e.target.value)} placeholder="Paste payload..." rows={3}
        className="w-full px-3 py-2 rounded-xl bg-muted/30 border border-border/30 text-sm font-mono text-foreground placeholder:text-muted-foreground/40 resize-none" />
      <div className="flex items-center gap-3 mt-2">
        <select value={encoding} onChange={e => setEncoding(e.target.value)} className="h-9 px-2 rounded-xl bg-muted/30 border border-border/30 text-xs text-foreground font-mono">
          {["Base64", "Hex", "UrlEncode"].map(e => <option key={e} value={e}>{e}</option>)}
        </select>
        <RunButton loading={loading} onClick={run} disabled={!input.trim()} />
      </div>
      {result && (
        <div className="relative mt-3">
          <pre className="p-3 rounded-xl bg-muted/20 border border-border/20 text-xs font-mono text-foreground break-all">{result.encoded}</pre>
          <CopyBtn text={result.encoded} />
        </div>
      )}
    </ToolPanel>
  );
};

// ─── Password Entropy ──────────────────────────────────────────
const PasswordTool = () => {
  const [pw, setPw] = useState("");
  const [result, setResult] = useState<PasswordEntropyDto | null>(null);
  const [loading, setLoading] = useState(false);

  const run = async () => {
    if (!pw) return;
    setLoading(true);
    try { const r = await secOpsUtilitiesApi.passwordEntropy({ password: pw }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };

  const strengthColors: Record<string, string> = { Weak: "text-red-400", Fair: "text-amber-400", Strong: "text-emerald-400", "Very Strong": "text-cyan-400" };

  return (
    <ToolPanel title="Password Entropy">
      <div className="flex items-end gap-3">
        <input type="text" value={pw} onChange={e => setPw(e.target.value)} placeholder="Enter password..."
          className="flex-1 h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono placeholder:text-muted-foreground/40"
          onKeyDown={e => e.key === "Enter" && run()} />
        <RunButton loading={loading} onClick={run} disabled={!pw} />
      </div>
      {result && (
        <div className="flex items-center gap-4 mt-3 p-3 rounded-xl bg-muted/20 border border-border/20">
          <div className="text-center">
            <p className="text-xl font-bold text-foreground font-mono">{result.entropy.toFixed(1)}</p>
            <p className="text-[10px] text-muted-foreground font-mono">bits</p>
          </div>
          <div className="h-8 w-px bg-border/30" />
          <div>
            <p className={`text-sm font-bold font-mono ${strengthColors[result.strength] || "text-foreground"}`}>{result.strength}</p>
            <p className="text-[10px] text-muted-foreground font-mono">Crack time: {result.estimatedCrackTime}</p>
          </div>
        </div>
      )}
    </ToolPanel>
  );
};

// ─── SSL Checker ───────────────────────────────────────────────
const SslTool = () => {
  const [hostname, setHostname] = useState("");
  const [result, setResult] = useState<SslCheckDto | null>(null);
  const [loading, setLoading] = useState(false);

  const run = async () => {
    if (!hostname.trim()) return;
    setLoading(true);
    try { const r = await secOpsUtilitiesApi.sslCheck({ hostname }); if (r.data.isSuccess) setResult(r.data.data); } catch {}
    setLoading(false);
  };

  return (
    <ToolPanel title="SSL Certificate Checker">
      <div className="flex items-end gap-3">
        <input type="text" value={hostname} onChange={e => setHostname(e.target.value)} placeholder="example.com"
          className="flex-1 h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono placeholder:text-muted-foreground/40"
          onKeyDown={e => e.key === "Enter" && run()} />
        <RunButton loading={loading} onClick={run} disabled={!hostname.trim()} />
      </div>
      {result && (
        <div className="mt-3 space-y-2">
          <div className={`p-3 rounded-xl border ${result.isValid ? "bg-emerald-500/5 border-emerald-500/15" : "bg-red-500/5 border-red-500/15"}`}>
            <div className="flex items-center gap-2 mb-2">
              <ShieldCheck className={`w-4 h-4 ${result.isValid ? "text-emerald-400" : "text-red-400"}`} />
              <span className={`text-xs font-bold font-mono ${result.isValid ? "text-emerald-400" : "text-red-400"}`}>{result.isValid ? "VALID" : "INVALID"}</span>
              <span className="text-xs text-muted-foreground font-mono ml-auto">{result.daysRemaining}d remaining</span>
            </div>
            <div className="grid grid-cols-2 gap-2 text-[10px] font-mono text-muted-foreground">
              <div><span className="text-foreground">Subject:</span> {result.subject}</div>
              <div><span className="text-foreground">Issuer:</span> {result.issuer}</div>
              <div><span className="text-foreground">Not Before:</span> {new Date(result.notBefore).toLocaleDateString()}</div>
              <div><span className="text-foreground">Not After:</span> {new Date(result.notAfter).toLocaleDateString()}</div>
            </div>
          </div>
        </div>
      )}
    </ToolPanel>
  );
};

// ─── Port Scanner ──────────────────────────────────────────────
const PortScanTool = () => {
  const [target, setTarget] = useState("127.0.0.1");
  const [startPort, setStartPort] = useState("1");
  const [endPort, setEndPort] = useState("1024");
  const [results, setResults] = useState<PortScanResultDto[]>([]);
  const [loading, setLoading] = useState(false);

  const run = async () => {
    setLoading(true);
    try {
      const r = await secOpsUtilitiesApi.portScan({ target, startPort: Number(startPort), endPort: Number(endPort) });
      if (r.data.isSuccess && r.data.data) setResults(r.data.data);
    } catch {}
    setLoading(false);
  };

  return (
    <ToolPanel title="Port Scanner">
      <div className="flex items-end gap-2">
        <div className="flex-1 space-y-1">
          <label className="text-[10px] text-muted-foreground font-mono">Target</label>
          <input type="text" value={target} onChange={e => setTarget(e.target.value)}
            className="w-full h-9 px-3 rounded-xl bg-muted/30 border border-border/30 text-sm text-foreground font-mono" />
        </div>
        <div className="w-20 space-y-1">
          <label className="text-[10px] text-muted-foreground font-mono">Start</label>
          <input type="number" value={startPort} onChange={e => setStartPort(e.target.value)}
            className="w-full h-9 px-2 rounded-xl bg-muted/30 border border-border/30 text-xs text-foreground font-mono" />
        </div>
        <div className="w-20 space-y-1">
          <label className="text-[10px] text-muted-foreground font-mono">End</label>
          <input type="number" value={endPort} onChange={e => setEndPort(e.target.value)}
            className="w-full h-9 px-2 rounded-xl bg-muted/30 border border-border/30 text-xs text-foreground font-mono" />
        </div>
        <RunButton loading={loading} onClick={run} />
      </div>
      {results.length > 0 && (
        <div className="mt-3 rounded-xl border border-border/20 overflow-hidden">
          <div className="grid grid-cols-3 gap-0 text-[10px] font-mono font-bold text-muted-foreground bg-muted/15 px-3 py-2 border-b border-border/15">
            <span>PORT</span><span>PROTOCOL</span><span>SERVICE</span>
          </div>
          {results.map((r, i) => (
            <div key={i} className="grid grid-cols-3 gap-0 text-xs font-mono text-foreground px-3 py-2 border-b border-border/10 last:border-0 hover:bg-muted/10">
              <span className="text-emerald-400">{r.port}</span>
              <span className="text-muted-foreground">{r.protocol}</span>
              <span>{r.serviceName}</span>
            </div>
          ))}
        </div>
      )}
    </ToolPanel>
  );
};

// ─── Shared UI ─────────────────────────────────────────────────
const ToolPanel = ({ title, children }: { title: string; children: React.ReactNode }) => (
  <motion.div initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} className="p-5 rounded-2xl bg-card/50 border border-border/30 space-y-3">
    <h3 className="text-sm font-bold text-foreground font-mono">{title}</h3>
    {children}
  </motion.div>
);

const RunButton = ({ loading, onClick, disabled }: { loading: boolean; onClick: () => void; disabled?: boolean }) => (
  <button onClick={onClick} disabled={loading || disabled}
    className="h-9 px-4 rounded-xl bg-primary text-primary-foreground text-xs font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors font-mono">
    {loading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : "Run"}
  </button>
);

const CopyBtn = ({ text }: { text: string }) => {
  const [copied, setCopied] = useState(false);
  return (
    <button onClick={() => { navigator.clipboard.writeText(text); setCopied(true); setTimeout(() => setCopied(false), 2000); }}
      className="absolute top-2 right-2 w-7 h-7 rounded-lg bg-card border border-border/30 flex items-center justify-center text-muted-foreground hover:text-foreground transition-colors">
      {copied ? <Check className="w-3 h-3 text-emerald-400" /> : <Copy className="w-3 h-3" />}
    </button>
  );
};

export default SecOpsUtilitiesPanel;

